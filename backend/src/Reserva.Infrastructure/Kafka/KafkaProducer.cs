using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Reserva.Infrastructure.Kafka;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IConfiguration config, ILogger<KafkaProducer> logger)
    {
        _logger = logger;

        var bootstrap = config["Kafka:BootstrapServers"]
            ?? throw new Exception("Kafka:BootstrapServers no configurado");

        _topic = config["Kafka:TopicReservas"]
            ?? throw new Exception("Kafka:TopicReservas no configurado");

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = bootstrap
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task ProduceAsync(string key, string value)
    {
        try
        {
            var msg = new Message<string, string>
            {
                Key = key,
                Value = value
            };

            var result = await _producer.ProduceAsync(_topic, msg);
            _logger.LogInformation(
                "Mensaje enviado a {Topic} partition {Partition}, offset {Offset}",
                result.Topic, result.Partition, result.Offset
            );
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Error enviando mensaje a Kafka");
            throw;
        }
    }
}

