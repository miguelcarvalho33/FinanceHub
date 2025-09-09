using System.Security.Cryptography;
using System.Text;
using FinanceHub.Core.Models;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using UglyToad.PdfPig;

namespace FinanceHub.Web.Services
{
    public class PdfProcessingService : IHostedService, IDisposable
    {
        private readonly ILogger<PdfProcessingService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly IEnumerable<IPdfParser> _parsers;
        private Timer? _timer;

        private readonly string _inputFolder;
        private readonly string _processedFolder;
        private readonly string _errorFolder;

        public PdfProcessingService(ILogger<PdfProcessingService> logger,
                                    IServiceScopeFactory scopeFactory,
                                    IConfiguration configuration,
                                    IEnumerable<IPdfParser> parsers)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _parsers = parsers;

            _inputFolder = _configuration.GetValue<string>("FolderPaths:Input")!;
            _processedFolder = _configuration.GetValue<string>("FolderPaths:Processed")!;
            _errorFolder = _configuration.GetValue<string>("FolderPaths:Error")!;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de Processamento de PDF a iniciar.");
            Directory.CreateDirectory(_inputFolder);
            Directory.CreateDirectory(_processedFolder);
            Directory.CreateDirectory(_errorFolder);
            _logger.LogInformation("Pasta de input a ser monitorizada: '{InputFolder}'", Path.GetFullPath(_inputFolder));
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            _logger.LogInformation("A verificar a pasta de input por novos ficheiros...");
            var pdfFiles = Directory.GetFiles(_inputFolder, "*.pdf");

            if (!pdfFiles.Any())
            {
                return;
            }

            _logger.LogInformation("Encontrados {FileCount} ficheiros para processar.", pdfFiles.Length);

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

                var categorizationService = scope.ServiceProvider.GetRequiredService<CategorizationService>();

                foreach (var filePath in pdfFiles)
                {
                    ProcessFile(filePath, dbContext, _parsers, categorizationService);
                }
            }
        }

        private void ProcessFile(string filePath, FinanceDbContext dbContext, IEnumerable<IPdfParser> parsers, CategorizationService categorizationService)
        {
            var fileName = Path.GetFileName(filePath);
            _logger.LogInformation("--- A processar '{FileName}' ---", fileName);
            try
            {
                string pdfText;
                using (var pdf = PdfDocument.Open(filePath))
                {
                    pdfText = GetTextWithLayout(pdf);
                }

                var parser = parsers.FirstOrDefault(p => p.CanParse(pdfText));
                if (parser == null)
                {
                    throw new Exception("Nenhum parser compatível foi encontrado para o conteúdo do ficheiro.");
                }

                _logger.LogInformation("Parser '{BankName}' selecionado.", parser.BankName);
                var transactions = parser.Parse(pdfText);
                _logger.LogInformation("Foram extraídas {Count} transações brutas.", transactions.Count);

                int newTransactionsCount = 0;
                foreach (var tx in transactions)
                {
                    categorizationService.ProcessTransaction(tx);

                    tx.Hash = CreateTransactionHash(tx);
                    var exists = dbContext.Transactions.Any(t => t.Hash == tx.Hash);
                    if (!exists)
                    {
                        dbContext.Transactions.Add(tx);
                        newTransactionsCount++;
                    }
                }

                dbContext.SaveChanges();
                _logger.LogInformation("{Count} novas transações foram guardadas.", newTransactionsCount);

                File.Move(filePath, Path.Combine(_processedFolder, fileName));
                _logger.LogInformation("Ficheiro movido para a pasta 'Processed'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERRO ao processar '{FileName}'. Ficheiro movido para a pasta 'Error'.", fileName);
                File.Move(filePath, Path.Combine(_errorFolder, fileName));
            }
        }

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

        private string CreateTransactionHash(Transaction tx)
        {
            var input = $"{tx.MovementDate:yyyy-MM-dd}-{tx.OriginalDescription}-{tx.Amount:F2}-{tx.Bank}";
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Serviço de Processamento de PDF a parar.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}