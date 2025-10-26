using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using System.Security.Cryptography;
using System.Text;
using UglyToad.PdfPig;

namespace FinanceHub.Web.Services
{
    public class PdfProcessingService : IHostedService, IDisposable
    {
        private readonly ILogger<PdfProcessingService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _inputPath;
        private readonly string _processedPath;
        private readonly string _errorPath;
        private Timer? _timer;

        public PdfProcessingService(ILogger<PdfProcessingService> logger, IConfiguration configuration, IServiceProvider serviceProvider, IHostEnvironment env)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            // Mantém a gestão de caminhos robusta
            var basePath = env.ContentRootPath;
            _logger.LogInformation("Base path for resolving folders is: {basePath}", basePath);

            var inputRelativePath = configuration.GetValue<string>("FolderPaths:Input")!;
            var processedRelativePath = configuration.GetValue<string>("FolderPaths:Processed")!;
            var errorRelativePath = configuration.GetValue<string>("FolderPaths:Error")!;

            _inputPath = Path.Combine(basePath, inputRelativePath);
            _processedPath = Path.Combine(basePath, processedRelativePath);
            _errorPath = Path.Combine(basePath, errorRelativePath);

            Directory.CreateDirectory(_inputPath);
            Directory.CreateDirectory(_processedPath);
            Directory.CreateDirectory(_errorPath);

            _logger.LogInformation("PDF Input Path is configured to: {path}", _inputPath);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PDF Processing Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            _logger.LogInformation("PDF Processing Service is checking for files...");
            var pdfFiles = Directory.GetFiles(_inputPath, "*.pdf");

            if (!pdfFiles.Any())
            {
                _logger.LogInformation("No PDF files found to process.");
                return;
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
                var pdfParsers = scope.ServiceProvider.GetRequiredService<IEnumerable<IPdfParser>>();
                var categorizationService = scope.ServiceProvider.GetRequiredService<CategorizationService>();

                foreach (var filePath in pdfFiles)
                {
                    var fileName = Path.GetFileName(filePath);
                    _logger.LogInformation("--- Processing file: {file} ---", fileName);
                    try
                    {
                        string text;
                        using (var pdf = PdfDocument.Open(filePath))
                        {
                            // Usa a TUA extração de texto inteligente
                            text = GetTextWithLayout(pdf);
                        }

                        var parser = pdfParsers.FirstOrDefault(p => p.CanParse(text));

                        if (parser == null)
                        {
                            throw new Exception("No suitable parser found for this PDF.");
                        }

                        // Try parse statement metadata if CGD Extrato Global
                        Core.Models.BankStatement? statement = null;
                        List<Core.Models.LoanPayment> parsedPayments = new();
                        if (parser is Web.Parsers.CgdExtratoGlobalParser cgd)
                        {
                            statement = cgd.ParseMetadata(text);
                            if (statement != null)
                            {
                                // Deduplicate by bank + statement number (or IBAN+Period)
                                var existingStmt = dbContext.BankStatements.FirstOrDefault(s =>
                                    (s.Bank == statement.Bank && s.StatementNumber == statement.StatementNumber) ||
                                    (s.Bank == statement.Bank && s.IBAN == statement.IBAN && s.Period == statement.Period));
                                if (existingStmt != null)
                                {
                                    statement = existingStmt;
                                }
                                else
                                {
                                    dbContext.BankStatements.Add(statement);
                                    dbContext.SaveChanges();
                                }

                                // Loans + payments
                                var (loans, payments) = cgd.ParseLoansAndPayments(text);
                                foreach (var loan in loans)
                                {
                                    loan.BankStatementId = statement.Id;
                                    var existsLoan = dbContext.Loans.Any(l => l.BankStatementId == loan.BankStatementId && l.LoanType == loan.LoanType && l.AccountNumber == loan.AccountNumber);
                                    if (!existsLoan)
                                    {
                                        dbContext.Loans.Add(loan);
                                        dbContext.SaveChanges();
                                    }
                                    else
                                    {
                                        // Attach existing
                                        var existingLoan = dbContext.Loans.First(l => l.BankStatementId == loan.BankStatementId && l.LoanType == loan.LoanType && l.AccountNumber == loan.AccountNumber);
                                        loan.Id = existingLoan.Id;
                                    }
                                }
                                foreach (var p in payments)
                                {
                                    // Attach to first loan (CGD extrato typically contains one home loan)
                                    var loanId = dbContext.Loans.Where(l => l.BankStatementId == statement.Id).Select(l => l.Id).FirstOrDefault();
                                    if (loanId != 0)
                                    {
                                        p.LoanId = loanId;
                                        var dupPay = dbContext.LoanPayments.Any(x => x.LoanId == p.LoanId && x.InstallmentNumber == p.InstallmentNumber && x.Description == p.Description && x.MovementDate == p.MovementDate && x.CapitalAmount == p.CapitalAmount && x.InterestAmount == p.InterestAmount);
                                        if (!dupPay)
                                        {
                                            dbContext.LoanPayments.Add(p);
                                        }
                                    }
                                }

                                // Direct Debits
                                var dds = cgd.ParseDirectDebits(text);
                                foreach (var dd in dds)
                                {
                                    dd.BankStatementId = statement.Id;
                                    var existsDd = dbContext.DirectDebits.Any(x => x.BankStatementId == dd.BankStatementId && x.CreationDate == dd.CreationDate && x.EntityNumber == dd.EntityNumber && x.AuthorizationNumber == dd.AuthorizationNumber);
                                    if (!existsDd)
                                    {
                                        dbContext.DirectDebits.Add(dd);
                                    }
                                }

                                // Cards
                                var cards = cgd.ParseCards(text);
                                foreach (var card in cards)
                                {
                                    card.BankStatementId = statement.Id;
                                    var existsCard = dbContext.Cards.Any(x => x.BankStatementId == card.BankStatementId && x.MaskedNumber == card.MaskedNumber);
                                    if (!existsCard)
                                    {
                                        dbContext.Cards.Add(card);
                                    }
                                }

                                dbContext.SaveChanges();
                            }
                        }

                        var transactions = parser.Parse(text);
                        _logger.LogInformation("Parsed {count} total transactions from the PDF.", transactions.Count);

                        var newTransactions = new List<Transaction>();
                        int duplicateCount = 0;

                        foreach (var transaction in transactions)
                        {
                            transaction.Hash = GenerateTransactionHash(transaction);
                            bool exists = dbContext.Transactions.Any(t => t.Hash == transaction.Hash);
                            if (!exists)
                            {
                                // Link to statement by date range if available
                                if (statement != null && !string.IsNullOrWhiteSpace(statement.Period))
                                {
                                    var parts = statement.Period.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                    if (parts.Length >= 3 && DateTime.TryParse(parts[0], out var from) && DateTime.TryParse(parts[2], out var to))
                                    {
                                        if (transaction.MovementDate >= from && transaction.MovementDate <= to)
                                        {
                                            transaction.BankStatementId = statement.Id;
                                        }
                                    }
                                }

                                categorizationService.ProcessTransaction(transaction);
                                newTransactions.Add(transaction);
                            }
                            else
                            {
                                duplicateCount++;
                            }
                        }

                        if (newTransactions.Any())
                        {
                            dbContext.Transactions.AddRange(newTransactions);
                            dbContext.SaveChanges();
                        }

                        // One-line summary
                        if (statement != null)
                        {
                            var ibanTail = string.IsNullOrWhiteSpace(statement.IBAN) ? "" : statement.IBAN[^4..];
                            _logger.LogInformation("Statement {bank} {stNo} ({per}, IBAN ****{tail}) | Tx: total={total}, new={@new}, dup={dup} | Loans={loans}, DDs={dds}, Cards={cards}",
                                statement.Bank, statement.StatementNumber, statement.Period, ibanTail, transactions.Count, newTransactions.Count, duplicateCount,
                                dbContext.Loans.Count(l => l.BankStatementId == statement.Id),
                                dbContext.DirectDebits.Count(d => d.BankStatementId == statement.Id),
                                dbContext.Cards.Count(c => c.BankStatementId == statement.Id));
                        }
                        else
                        {
                            _logger.LogInformation("No statement metadata. Tx: total={total}, new={@new}, dup={dup}", transactions.Count, newTransactions.Count, duplicateCount);
                        }

                        MoveFile(filePath, _processedPath);
                        _logger.LogInformation("File '{file}' processed successfully and moved.", fileName);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file {file}", fileName);
                        MoveFile(filePath, _errorPath);
                    }
                }
            }
        }

        // O TEU MÉTODO DE EXTRAÇÃO DE TEXTO, mantido por ser mais eficaz
        private string GetTextWithLayout(PdfDocument pdf)
        {
            var fullText = new StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                var words = page.GetWords();
                if (!words.Any()) continue;

                var groupedLines = words
                    .GroupBy(w => Math.Round(w.BoundingBox.Bottom, 2))
                    .OrderByDescending(g => g.Key);

                foreach (var group in groupedLines)
                {
                    var lineText = string.Join(" ", group.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));
                    fullText.AppendLine(lineText);
                }
                fullText.AppendLine();
            }
            return fullText.ToString();
        }

        private void MoveFile(string sourcePath, string destinationFolder)
        {
            var destinationPath = Path.Combine(destinationFolder, Path.GetFileName(sourcePath));
            File.Move(sourcePath, destinationPath, true);
        }

        private string GenerateTransactionHash(Transaction transaction)
        {
            var input = $"{transaction.MovementDate:yyyy-MM-dd}-{transaction.Amount:F2}-{transaction.OriginalDescription}-{transaction.Bank}";
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PDF Processing Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}