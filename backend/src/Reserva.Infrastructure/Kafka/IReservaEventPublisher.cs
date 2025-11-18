using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka;

public interface IReservaEventPublisher
{
    Task PublicarReservaCreadaAsync(ReservaEntity reserva);
}
