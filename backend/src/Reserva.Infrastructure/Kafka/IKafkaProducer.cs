using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka;

public interface IKafkaProducer
{
    Task ProduceAsync(string key, string value);
}
