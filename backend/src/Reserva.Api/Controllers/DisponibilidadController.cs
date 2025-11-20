using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Core.Models;
using Reserva.Infrastructure.Data;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DisponibilidadController(AppDbContext db)
        {
            _db = db;
        }

        // GET api/disponibilidad?espacioId=1&start=2025-01-01&end=2025-01-31
        [HttpGet]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> GetDisponibilidad(
            int espacioId,
            DateTime start,
            DateTime end)
        {
            if (start >= end)
                return BadRequest("Rango de fechas inválido.");

            // Cargar reservas existentes
            var reservas = await _db.Reservas
                .Where(r => r.EspacioId == espacioId &&
                            r.FechaFin > start &&
                            r.FechaInicio < end)
                .ToListAsync();

            // El frontend construirá el calendario,
            // aquí devolvemos solo las ocupaciones.
            return Ok(new
            {
                espacioId,
                rangosOcupados = reservas.Select(r => new
                {
                    inicio = r.FechaInicio,
                    fin = r.FechaFin
                })
            });
        }
    }
}
