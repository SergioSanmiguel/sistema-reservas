using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Reserva.Infrastructure.Data;
using Reserva.Core.Models;
using System.Security.Claims;

namespace Reserva.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            if (await _db.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return Conflict(new { message = "Email already registered." });

            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new Usuario { Email = dto.Email, PasswordHash = hashed, Rol = "Usuario" };
            _db.Usuarios.Add(user);
            await _db.SaveChangesAsync();

            return Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.Rol });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var user = await _db.Usuarios.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized();

            var jwt = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwt.GetValue<string>("Key")!);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Rol)
                }),
                Expires = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpireMinutes")),
                Issuer = jwt.GetValue<string>("Issuer"),
                Audience = jwt.GetValue<string>("Audience"),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { token = tokenString, role = user.Rol });
        }
    }

    // DTOs
    public record UserRegisterDto(string Email, string Password);
    public record UserLoginDto(string Email, string Password);
}
