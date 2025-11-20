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

            // Nuevo topic final del proyecto
            _topicFinalizacion = _config["Kafka:TopicReservaListadaParaFinalizacion"]
                ?? throw new Exception("Kafka:TopicReservaListadaParaFinalizacion no configurado");

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _consumer.Subscribe(_topicFinalizacion);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("KafkaConsumerService iniciado. Escuchando topic: {Topic}", _topicFinalizacion);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(stoppingToken);

                    if (cr == null)
                        continue;

                    _logger.LogInformation(
                        "Mensaje recibido de {Topic} → Key: {Key}, Value: {Value}",
                        _topicFinalizacion,
                        cr.Message.Key,
                        cr.Message.Value
                    );

                    // 🟦 Deserializar el evento real
                    var evento = JsonSerializer.Deserialize<EventoReservaListadaParaFinalizacion>(cr.Message.Value);

                    if (evento == null)
                    {
                        _logger.LogWarning("No se pudo deserializar el evento recibido");
                        continue;
                    }

                    // 🟧 Llamar a la lógica de dominio (lo conectarás después)
                    await ProcesarEventoAsync(evento);
                }
                catch (OperationCanceledException)
                {
                    // Se cancela correctamente al apagar la app
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error procesando mensaje de Kafka");
                }

                // Pequeño delay opcional para no saturar CPU
                await Task.Delay(10, stoppingToken);
            }
        }

        private Task ProcesarEventoAsync(EventoReservaListadaParaFinalizacion evento)
        {
            _logger.LogInformation("Procesando evento → ReservaId: {Id}", evento.ReservaId);



            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }

    // Modelo del evento consumido desde Kafka
    public class EventoReservaListadaParaFinalizacion
    {
        public int ReservaId { get; set; }
        public DateTime FechaLimite { get; set; }
        public string EstadoActual { get; set; } = "";
    }
}

