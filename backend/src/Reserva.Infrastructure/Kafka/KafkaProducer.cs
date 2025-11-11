using Confluent.Kafka;
using System.Text.Json;
using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic = "reservas-topic";

    public KafkaProducer(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublicarReservaCreadaAsync(ReservaEntity reserva)
    {
        var json = JsonSerializer.Serialize(reserva);
        await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
    }
}
