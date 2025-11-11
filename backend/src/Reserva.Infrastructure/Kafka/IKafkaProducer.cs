using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task PublicarReservaCreadaAsync(ReservaEntity reserva);
}
