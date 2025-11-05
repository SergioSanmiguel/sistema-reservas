using Microsoft.EntityFrameworkCore;
using Reserva.Api.Data;
using Reserva.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Cargar configuración de conexión a PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

// 🔹 Registrar DbContext para EF Core con PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// 🔹 Agregar controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 🔹 Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // opcional en local
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// 🔹 Endpoint de prueba rápido
app.MapGet("/api/health", () => Results.Ok("✅ API de Reservas funcionando correctamente."));

app.Run();

