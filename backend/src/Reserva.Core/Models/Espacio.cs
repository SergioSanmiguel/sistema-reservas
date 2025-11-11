namespace Reserva.Core.Models
{
    public class Espacio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Disponible { get; set; } = true;
    }
}
