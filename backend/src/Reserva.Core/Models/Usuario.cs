using System.ComponentModel.DataAnnotations;

namespace Reserva.Core.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string Rol { get; set; } = "Usuario";
    }
}
