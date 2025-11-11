using Microsoft.EntityFrameworkCore;
using Reserva.Core.Models;

namespace Reserva.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ReservaEntity> Reservas { get; set; }
        public DbSet<Espacio> Espacios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}


