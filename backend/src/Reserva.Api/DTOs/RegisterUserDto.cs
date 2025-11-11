using System.ComponentModel.DataAnnotations;

namespace Reserva.Api.DTOs
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; set; }

        [Required]
        [RegularExpression("^(usuario|admin)$", ErrorMessage = "Rol inválido")]
        public string Rol { get; set; } = "usuario";
    }
}
