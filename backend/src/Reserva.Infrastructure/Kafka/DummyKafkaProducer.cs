using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka
{
    public class DummyKafkaProducer : IKafkaProducer
    {
        private readonly ILogger<DummyKafkaProducer> _logger;

        public DummyKafkaProducer(ILogger<DummyKafkaProducer> logger)
        {
            _logger = logger;
        }

        public Task PublicarReservaCreadaAsync(ReservaEntity reserva)
        {
            _logger.LogInformation(
                "[KafkaDummy] Evento publicado -> Reserva ID: {Id}, Usuario: {UsuarioId}, Espacio: {EspacioId}, Inicio: {Inicio}, Fin: {Fin}",
                reserva.Id,
                reserva.IdUsuario,
                reserva.IdEspacio,
                reserva.FechaInicio,
                reserva.FechaFin
            );

            // No hace nada real, solo simula el envío
            return Task.CompletedTask;
        }
    }
}
