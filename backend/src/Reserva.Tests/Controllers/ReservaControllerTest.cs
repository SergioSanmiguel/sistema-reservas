using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using Reserva.Api;
using Reserva.Infrastructure.Kafka;
using System.Text.Json;

namespace Reserva.Tests.Api
{
    public class ReservaControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly IKafkaProducer _kafkaProducer;

        public ReservaControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Añadir token JWT para autenticación
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", factory.TestUserToken);

            // Obtener el producer mock si lo has registrado en la factory
            _kafkaProducer = _factory.Services.GetService(typeof(IKafkaProducer)) as IKafkaProducer;
        }

        [Fact]
        public async Task Get_Reservas_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/reservas");
            response.EnsureSuccessStatusCode();

            var reservas = await response.Content.ReadFromJsonAsync<object[]>();
            Assert.NotNull(reservas);
        }

        [Fact]
        public async Task Post_Reserva_ReturnsCreated_AndPublishesKafkaEvent()
        {
            var nuevaReserva = new
            {
                EspacioId = 1,
                FechaInicio = DateTime.UtcNow.AddHours(1),
                FechaFin = DateTime.UtcNow.AddHours(2)
            };

            var response = await _client.PostAsJsonAsync("/api/reservas", nuevaReserva);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            // Validación básica de Kafka (si usas un mock que guarda mensajes)
            if (_kafkaProducer is MockKafkaProducer mock)
            {
                var lastMessage = mock.SentMessages.LastOrDefault();
                Assert.NotNull(lastMessage);

                // Opcional: validar el topic y contenido
                Assert.Equal("reserva-creada", lastMessage.Topic);
                var payload = JsonSerializer.Deserialize<Dictionary<string, object>>(lastMessage.Message);
                Assert.Equal(1, (int)payload["EspacioId"]);
            }
        }
    }
}





