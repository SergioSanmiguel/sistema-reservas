using Microsoft.EntityFrameworkCore;
using Reserva.Api.Models;

namespace Reserva.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ReservaEntity> Reservas { get; set; }
    }
}
