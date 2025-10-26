using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Parsers;
using FinanceHub.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure logging to use log4net
builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinanceDbContext>(options =>
 options.UseSqlite(connectionString));

// Parsers
builder.Services.AddSingleton<IPdfParser, SantanderParser>();
builder.Services.AddSingleton<IPdfParser, CgdParser>();
builder.Services.AddScoped<IPdfParser, CgdExtratoGlobalParser>();

builder.Services.AddScoped<CategorizationService>();

builder.Services.AddScoped<VcfParserService>();

builder.Services.AddHostedService<PdfProcessingService>();

var app = builder.Build();

// Ensure DB is migrated/created
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