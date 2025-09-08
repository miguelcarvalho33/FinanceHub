using Microsoft.EntityFrameworkCore;
using FinanceHub.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// --- Início da Configuração dos Serviços ---

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