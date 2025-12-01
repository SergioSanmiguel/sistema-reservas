using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Reserva.Infrastructure.Kafka
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly IConfiguration _config;
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topicFinalizacion;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            var bootstrap = _config["Kafka:BootstrapServers"]
                ?? throw new Exception("Kafka:BootstrapServers no configurado");

            var groupId = _config["Kafka:GroupId"]
                ?? throw new Exception("Kafka:GroupId no configurado");

            _topicFinalizacion = _config["Kafka:TopicReservaListadaParaFinalizacion"]
                ?? throw new Exception("Kafka:TopicReservaListadaParaFinalizacion no configurado");

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,   // ⬅ Mejor control
                AllowAutoCreateTopics = true,
                EnablePartitionEof = true
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig)
                .SetErrorHandler((_, e) =>
                {
                    logger.LogError("Kafka error: {Reason}", e.Reason);
                })
                .SetPartitionsAssignedHandler((_, partitions) =>
                {
                    logger.LogInformation("Particiones asignadas: {Parts}", string.Join(", ", partitions));
                })
                .SetPartitionsRevokedHandler((_, partitions) =>
                {
                    logger.LogWarning("Particiones revocadas: {Parts}", string.Join(", ", partitions));
                })
                .Build();

            _consumer.Subscribe(_topicFinalizacion);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaConsumerService iniciado. Topic escuchado: {Topic}", _topicFinalizacion);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(stoppingToken);

                    if (result == null || result.IsPartitionEOF)
                        continue;

                    _logger.LogInformation("Mensaje recibido → Key: {Key}, Offset: {Offset}",
                        result.Message.Key, result.Offset);

                    var evento = JsonSerializer.Deserialize<EventoReservaListadaParaFinalizacion>(result.Message.Value);

                    if (evento == null)
                    {
                        _logger.LogWarning("Evento nulo o no deserializable");
                        continue;
                    }

                    // ---- Lógica de procesamiento ----
                    await ProcesarEventoAsync(evento);

                    // ---- Commit manual segurísimo ----
                    _consumer.Commit(result);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error de consumo Kafka");
                }
                catch (OperationCanceledException)
                {
                    // OK: servicio apagándose
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando mensaje");
                }

                await Task.Delay(5, stoppingToken);
            }
        }

        private Task ProcesarEventoAsync(EventoReservaListadaParaFinalizacion evento)
        {
            _logger.LogInformation(
                "Procesando evento → ReservaId: {Id}, FechaLimite: {Fecha}, Estado: {Estado}",
                evento.ReservaId,
                evento.FechaLimite,
                evento.EstadoActual
            );

            // Aquí pongo tel dominio más adelante -> llamar servicio de dominio
            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            try
            {
                _consumer.Close(); // Limpia offsets y suscripciones
            }
            finally
            {
                _consumer.Dispose();
            }

            base.Dispose();
        }
    }

    public class EventoReservaListadaParaFinalizacion
    {
        public int ReservaId { get; set; }
        public DateTime FechaLimite { get; set; }
        public string EstadoActual { get; set; } = "";
    }
}


