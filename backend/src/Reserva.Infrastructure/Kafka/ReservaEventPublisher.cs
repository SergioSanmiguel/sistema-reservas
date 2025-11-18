using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reserva.Core.Models;

namespace Reserva.Infrastructure.Kafka;

public class ReservaEventPublisher : IReservaEventPublisher
{
    private readonly IKafkaProducer _producer;
    private readonly ILogger<ReservaEventPublisher> _logger;

    public ReservaEventPublisher(IKafkaProducer producer, ILogger<ReservaEventPublisher> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task PublicarReservaCreadaAsync(ReservaEntity reserva)
    {
        var json = JsonSerializer.Serialize(reserva);

        await _producer.ProduceAsync("reserva-creada", json);

        _logger.LogInformation("Evento ReservaCreada publicado: {Id}", reserva.Id);
    }
}
