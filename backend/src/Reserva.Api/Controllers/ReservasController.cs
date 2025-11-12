using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Core.Models;
using Reserva.Infrastructure.Data;
using Reserva.Infrastructure.Kafka;
using System.Security.Claims;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IKafkaProducer _producer;

        public ReservasController(AppDbContext db, IKafkaProducer producer)
        {
            _db = db;
            _producer = producer;
        }

        // GET: api/reservas
        [HttpGet]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var reservas = await _db.Reservas
                .Include(r => r.Espacio)
                .Include(r => r.Usuario)
                .ToListAsync();

            return Ok(reservas);
        }

        // GET: api/reservas/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var reserva = await _db.Reservas
                .Include(r => r.Espacio)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            return Ok(reserva);
        }

        // POST: api/reservas
        [HttpPost]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> Create([FromBody] ReservaEntity reserva)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            reserva.IdUsuario = userId;

            var solapada = await _db.Reservas.AnyAsync(r =>
                r.IdEspacio == reserva.IdEspacio &&
                ((reserva.FechaInicio >= r.FechaInicio && reserva.FechaInicio < r.FechaFin) ||
                 (reserva.FechaFin > r.FechaInicio && reserva.FechaFin <= r.FechaFin)));

            if (solapada)
                return BadRequest(new { message = "El espacio ya está reservado en ese horario." });

            _db.Reservas.Add(reserva);
            await _db.SaveChangesAsync();

            await _producer.PublicarReservaCreadaAsync(reserva);

            return CreatedAtAction(nameof(GetById), new { id = reserva.Id }, reserva);
        }

        // DELETE: api/reservas/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var reserva = await _db.Reservas.FindAsync(id);
            if (reserva == null)
                return NotFound();

            _db.Reservas.Remove(reserva);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}

