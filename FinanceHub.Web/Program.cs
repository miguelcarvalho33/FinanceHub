using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Parsers;
using FinanceHub.Web.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- Início da Configuração dos Serviços ---
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// 1. Adicionar os serviços para a API e o Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Diz à aplicação como encontrar a connection string no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Regista o DbContext, dizendo-lhe para usar SQLite com a connection string
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(connectionString));
// ********************************************************************


builder.Services.AddSingleton<IPdfParser, SantanderParser>(); // MUDADO DE AddScoped
builder.Services.AddSingleton<IPdfParser, CgdParser>();     // MUDADO DE AddScoped

// Registar o nosso serviço de background
builder.Services.AddHostedService<PdfProcessingService>();

var app = builder.Build();

// Configurar o pipeline de pedidos HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();