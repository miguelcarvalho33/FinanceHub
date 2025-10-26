using FinanceHub.Core.Common.Helpers;
using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Data.Repositories;
using Microsoft.EntityFrameworkCore;
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
            var basePath = env.ContentRootPath;
            _inputPath = Path.Combine(basePath, configuration.GetValue<string>("FolderPaths:Input")!);
            _processedPath = Path.Combine(basePath, configuration.GetValue<string>("FolderPaths:Processed")!);
            _errorPath = Path.Combine(basePath, configuration.GetValue<string>("FolderPaths:Error")!);
            FileHelper.EnsureDirectory(_inputPath);
            FileHelper.EnsureDirectory(_processedPath);
            FileHelper.EnsureDirectory(_errorPath);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PDF Processing Service is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            var pdfFiles = Directory.GetFiles(_inputPath, "*.pdf");
            if (!pdfFiles.Any()) return;

            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
            var pdfParsers = scope.ServiceProvider.GetRequiredService<IEnumerable<IPdfParser>>();
            var categorizationService = scope.ServiceProvider.GetRequiredService<CategorizationService>();
            var stmtRepo = scope.ServiceProvider.GetRequiredService<Core.Application.Interfaces.IBankStatementRepository>();
            var txRepo = scope.ServiceProvider.GetRequiredService<Core.Application.Interfaces.ITransactionRepository>();

            foreach (var filePath in pdfFiles)
            {
                var fileName = Path.GetFileName(filePath);
                try
                {
                    string text;
                    using (var pdf = PdfDocument.Open(filePath))
                    {
                        text = GetTextWithLayout(pdf);
                    }
                    var parser = pdfParsers.FirstOrDefault(p => p.CanParse(text));
                    if (parser == null) throw new Exception("No suitable parser found for this PDF.");

                    BankStatement? statement = null;
                    if (parser is Parsers.CgdExtratoGlobalParser cgd)
                    {
                        statement = cgd.ParseMetadata(text);
                        if (statement != null)
                        {
                            statement = stmtRepo.AddOrGetAsync(statement).GetAwaiter().GetResult();
                            // loans/direct debits/cards handled as before
                            var (loans, payments) = cgd.ParseLoansAndPayments(text);
                            foreach (var loan in loans)
                            {
                                loan.BankStatementId = statement.Id;
                                if (!dbContext.Loans.Any(l => l.BankStatementId == loan.BankStatementId && l.LoanType == loan.LoanType && l.AccountNumber == loan.AccountNumber))
                                    dbContext.Loans.Add(loan);
                            }
                            foreach (var p in payments)
                            {
                                var loanId = dbContext.Loans.Where(l => l.BankStatementId == statement.Id).Select(l => l.Id).FirstOrDefault();
                                if (loanId != 0)
                                {
                                    p.LoanId = loanId;
                                    var dup = dbContext.LoanPayments.Any(x => x.LoanId == p.LoanId && x.InstallmentNumber == p.InstallmentNumber && x.Description == p.Description && x.MovementDate == p.MovementDate && x.CapitalAmount == p.CapitalAmount && x.InterestAmount == p.InterestAmount);
                                    if (!dup) dbContext.LoanPayments.Add(p);
                                }
                            }
                            foreach (var dd in cgd.ParseDirectDebits(text))
                            {
                                dd.BankStatementId = statement.Id;
                                if (!dbContext.DirectDebits.Any(x => x.BankStatementId == dd.BankStatementId && x.CreationDate == dd.CreationDate && x.EntityNumber == dd.EntityNumber && x.AuthorizationNumber == dd.AuthorizationNumber))
                                    dbContext.DirectDebits.Add(dd);
                            }
                            foreach (var card in cgd.ParseCards(text))
                            {
                                card.BankStatementId = statement.Id;
                                if (!dbContext.Cards.Any(x => x.BankStatementId == card.BankStatementId && x.MaskedNumber == card.MaskedNumber))
                                    dbContext.Cards.Add(card);
                            }
                            dbContext.SaveChanges();
                        }
                    }
                    else if (parser is Parsers.SantanderParser santander)
                    {
                        statement = santander.ParseMetadata(text);
                        statement = stmtRepo.AddOrGetAsync(statement).GetAwaiter().GetResult();
                    }

                    var transactions = parser.Parse(text);
                    var range = TryGetPeriodRange(statement?.Period);
                    var newTransactions = new List<Transaction>();
                    var duplicateCount = 0;
                    foreach (var transaction in transactions)
                    {
                        transaction.Hash = HashingHelper.ComputeTransactionHash(transaction);
                        if (!txRepo.ExistsByHashAsync(transaction.Hash).GetAwaiter().GetResult())
                        {
                            if (statement != null && range.HasValue)
                            {
                                var (from, to) = range.Value;
                                if (transaction.MovementDate >= from && transaction.MovementDate <= to)
                                    transaction.BankStatementId = statement.Id;
                            }
                            categorizationService.ProcessTransaction(transaction);
                            newTransactions.Add(transaction);
                        }
                        else duplicateCount++;
                    }
                    if (newTransactions.Any())
                    {
                        txRepo.AddNewAsync(newTransactions).GetAwaiter().GetResult();
                    }
                    var ibanTail = string.IsNullOrWhiteSpace(statement?.IBAN) ? string.Empty : statement!.IBAN[^4..];
                    _logger.LogInformation("Processed {Total} tx (new={New}, dup={Dup}) for {Bank} Extrato {StmtNo} ({Period}, IBAN ****{IbanTail})",
                        transactions.Count, newTransactions.Count, duplicateCount, statement?.Bank, statement?.StatementNumber, statement?.Period, ibanTail);

                    FileHelper.MoveFile(filePath, _processedPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file {File}", fileName);
                    FileHelper.MoveFile(filePath, _errorPath);
                }
            }
        }

        private static (DateTime from, DateTime to)? TryGetPeriodRange(string? period)
        {
            if (string.IsNullOrWhiteSpace(period)) return null;
            // Expect formats like "YYYY-MM-DD a YYYY-MM-DD"
            var parts = period.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 && DateTime.TryParse(parts[0], out var from) && DateTime.TryParse(parts[2], out var to))
            {
                return (from, to);
            }
            return null;
        }

        private string GetTextWithLayout(UglyToad.PdfPig.PdfDocument pdf)
        {
            var fullText = new System.Text.StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                var words = page.GetWords();
                if (!words.Any()) continue;
                var groupedLines = words.GroupBy(w => Math.Round(w.BoundingBox.Bottom, 2)).OrderByDescending(g => g.Key);
                foreach (var group in groupedLines)
                {
                    var lineText = string.Join(" ", group.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));
                    fullText.AppendLine(lineText);
                }
                fullText.AppendLine();
            }
            return fullText.ToString();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("PDF Processing Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}