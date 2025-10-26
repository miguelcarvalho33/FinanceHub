using FinanceHub.Core.Application.Interfaces;
using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Data.Repositories;
using FinanceHub.Web.Parsers;
using FinanceHub.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

// MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FinanceDbContext>(options => options.UseSqlite(connectionString));

// Parsers
builder.Services.AddSingleton<IPdfParser, SantanderParser>();
builder.Services.AddSingleton<IPdfParser, CgdParser>();
builder.Services.AddScoped<IPdfParser, CgdExtratoGlobalParser>();

// Domain services
builder.Services.AddScoped<CategorizationService>();

// Repositories
builder.Services.AddScoped<IBankStatementRepository, BankStatementRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Hosted service
builder.Services.AddHostedService<PdfProcessingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
 var db = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();
 db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();