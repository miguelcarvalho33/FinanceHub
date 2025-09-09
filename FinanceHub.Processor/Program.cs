using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FinanceHub.Processor.Data;
using Microsoft.EntityFrameworkCore;
using FinanceHub.Core.Parsing;
using FinanceHub.Processor.Parsers;
using UglyToad.PdfPig;
using FinanceHub.Processor.Services;
using System.Security.Cryptography;
using System.Text;
using FinanceHub.Core.Models;

var builder = Host.CreateApplicationBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinanceDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<CategorizationService>();

var host = builder.Build();

Console.WriteLine("Aplicação configurada. A iniciar o processamento...");

using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<FinanceDbContext>();
    var config = services.GetRequiredService<IConfiguration>();

    dbContext.Database.EnsureCreated();
    Console.WriteLine("Base de dados verificada/criada com sucesso!");

    var categorizationService = services.GetRequiredService<CategorizationService>();

    var inputFolder = config.GetValue<string>("FolderPaths:Input")!;
    var processedFolder = config.GetValue<string>("FolderPaths:Processed")!;
    var errorFolder = config.GetValue<string>("FolderPaths:Error")!;
    Directory.CreateDirectory(inputFolder);
    Directory.CreateDirectory(processedFolder);
    Directory.CreateDirectory(errorFolder);

    var parsers = new List<IPdfParser> { new SantanderParser(), new CgdParser() };
    var pdfFiles = Directory.GetFiles(inputFolder, "*.pdf");
    Console.WriteLine($"Encontrados {pdfFiles.Length} ficheiros para processar.");

    foreach (var filePath in pdfFiles)
    {
        var fileName = Path.GetFileName(filePath);
        Console.WriteLine($"\n--- A processar '{fileName}' ---");
        try
        {
            string pdfText = "";
            using (var pdf = PdfDocument.Open(filePath))
            {
                foreach (var page in pdf.GetPages()) { pdfText += page.Text + "\n"; }
            }

            var parser = parsers.FirstOrDefault(p => p.CanParse(pdfText));
            if (parser == null) throw new Exception("Nenhum parser compatível foi encontrado.");

            Console.WriteLine($"Parser '{parser.BankName}' selecionado.");
            var transactions = parser.Parse(pdfText);
            Console.WriteLine($"Foram extraídas {transactions.Count} transações brutas.");

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
            Console.WriteLine($"{newTransactionsCount} novas transações foram guardadas na base de dados.");

            File.Move(filePath, Path.Combine(processedFolder, fileName));
            Console.WriteLine("Ficheiro processado e movido com sucesso.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO ao processar '{fileName}': {ex.Message}");
            File.Move(filePath, Path.Combine(errorFolder, fileName));
        }
    }
}

Console.WriteLine("\nProcessamento concluído.");

string CreateTransactionHash(Transaction tx)
{
    var input = $"{tx.MovementDate:yyyy-MM-dd}-{tx.OriginalDescription}-{tx.Amount:F2}-{tx.Bank}";
    using var sha256 = SHA256.Create();
    var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
    return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
}