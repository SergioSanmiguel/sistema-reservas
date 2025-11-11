using System.ComponentModel.DataAnnotations.Schema;

namespace Reserva.Core.Models
{
    public class ReservaEntity
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Activa";

        // Relaciones
        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [ForeignKey("Espacio")]
        public int EspacioId { get; set; }
        public Espacio? Espacio { get; set; }
    }
}


