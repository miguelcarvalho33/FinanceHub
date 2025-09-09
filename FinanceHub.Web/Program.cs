using FinanceHub.Core.Parsing;
using FinanceHub.Web.Data;
using FinanceHub.Web.Parsers;
using FinanceHub.Web.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddSingleton<IPdfParser, SantanderParser>();
builder.Services.AddSingleton<IPdfParser, CgdParser>();

builder.Services.AddScoped<CategorizationService>();

builder.Services.AddHostedService<PdfProcessingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();