using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Reserva.Infrastructure.Kafka;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IConfiguration _config;
    private IConsumer<string, string> _consumer;
    private readonly string _topic;

    public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration config)
    {
        _logger = logger;
        _config = config;

        var bootstrap = _config["Kafka:BootstrapServers"]
            ?? throw new Exception("Kafka:BootstrapServers no configurado");

        var groupId = _config["Kafka:GroupId"]
            ?? throw new Exception("Kafka:GroupId no configurado");

        _topic = _config["Kafka:TopicReservas"]
            ?? throw new Exception("Kafka:TopicReservas no configurado");

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = bootstrap,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        _consumer.Subscribe(_topic);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka Consumer escuchando topic: {Topic}", _topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var cr = _consumer.Consume(stoppingToken);

                if (cr != null)
                {
                    _logger.LogInformation(
                        "Mensaje recibido → Key: {Key}, Value: {Value}, Offset: {Offset}",
                        cr.Message.Key, cr.Message.Value, cr.Offset
                    );

                    // Aquí procesas el mensaje real (guardar en DB, lógica, eventos…)
                }
            }
            catch (OperationCanceledException)
            {
                // normal cuando se apaga la app
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en KafkaConsumer");
            }

            await Task.Delay(10, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _consumer?.Close();
        _consumer?.Dispose();
        base.Dispose();
    }
}
