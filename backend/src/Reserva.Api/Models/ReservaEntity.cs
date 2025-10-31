using System;

namespace Reserva.Api.Models
{
    public class ReservaEntity
    {
        public int Id { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Espacio { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Activa";
    }
}

