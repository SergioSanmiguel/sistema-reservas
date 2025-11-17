using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reserva.Api;
using Reserva.Infrastructure.Data;
using Reserva.Core.Models;
using System.Linq;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Reserva.Tests
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _defaultKey = "MiClaveSuperSecretaParaTests123!";

        // Tokens de prueba por rol
        public string TestUserToken { get; private set; } = "";
        public string TestAdminToken { get; private set; } = "";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Eliminar DbContext real registrado
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Añadir DbContext en memoria para testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Construir ServiceProvider
                var sp = services.BuildServiceProvider();

                // Limpiar y seed DB
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                // Seed inicial: al menos un espacio
                if (!db.Espacios.Any())
                {
                    db.Espacios.Add(new Espacio { Nombre = "Sala 1" });
                    db.SaveChanges();
                }

                // Crear tokens de prueba para Usuario y Admin
                TestUserToken = GenerateToken(1, "Usuario", "test@user.com");
                TestAdminToken = GenerateToken(2, "Admin", "admin@user.com");
            });
        }

        private string GenerateToken(int userId, string role, string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_defaultKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}







