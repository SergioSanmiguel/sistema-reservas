using System.ComponentModel.DataAnnotations;

namespace Reserva.Api.DTOs
{
    public class LoginUserDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        public required string Password { get; set; }
    }
}

