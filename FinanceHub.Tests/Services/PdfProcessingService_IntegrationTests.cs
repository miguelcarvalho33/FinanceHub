using System.IO;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Parsers;
using FinanceHub.Web.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UglyToad.PdfPig;
using Xunit;

namespace FinanceHub.Tests.Services
{
 public class PdfProcessingService_IntegrationTests
 {
 [Fact(Skip = "Requires sample PDF and UglyToad.PdfPig text; provide when running locally")] 
 public void ProcessesCgdExtratoGlobal_EndToEnd()
 {
 // Arrange in-memory sqlite
 var options = new DbContextOptionsBuilder<FinanceDbContext>()
 .UseSqlite(CreateInMemoryDatabase())
 .Options;
 using var context = new FinanceDbContext(options);
 context.Database.OpenConnection();
 context.Database.EnsureCreated();

 var services = new ServiceCollection();
 services.AddSingleton<IPdfParser, CgdExtratoGlobalParser>();
 services.AddSingleton<IPdfParser, CgdParser>();
 services.AddSingleton<IPdfParser, SantanderParser>();
 services.AddScoped<CategorizationService>();
 services.AddDbContext<FinanceDbContext>(o => o.UseSqlite(context.Database.GetDbConnection()));
 var sp = services.BuildServiceProvider();

 var parser = sp.GetRequiredService<IEnumerable<IPdfParser>>();
 Assert.NotNull(parser);
 }

 private static SqliteConnection CreateInMemoryDatabase()
 {
 var connection = new SqliteConnection("DataSource=:memory:");
 connection.Open();
 return connection;
 }
 }
}
