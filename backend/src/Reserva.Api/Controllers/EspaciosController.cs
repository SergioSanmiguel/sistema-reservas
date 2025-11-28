using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.Core.Models;
using Reserva.Infrastructure.Data;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EspaciosController : ControllerBase
    {
        private readonly AppDbContext _db;

        public EspaciosController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/espacios
        [HttpGet]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var espacios = await _db.Espacios.ToListAsync();
            return Ok(espacios);
        }

        // GET: api/espacios/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Usuario,Admin")]
        public async Task<IActionResult> GetById(int id)
        {
            var espacio = await _db.Espacios.FirstOrDefaultAsync(e => e.Id == id);

            if (espacio == null)
                return NotFound();

            return Ok(espacio);
        }
    }
}

