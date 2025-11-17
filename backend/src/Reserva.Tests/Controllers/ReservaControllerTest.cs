using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using Reserva.Api;

namespace Reserva.Tests.Api
{
    public class ReservaControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public ReservaControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();

            // Añadir token JWT para autenticación
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", factory.TestUserToken);
        }

        [Fact]
        public async Task Get_Reservas_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/reservas");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_Reserva_ReturnsCreated()
        {
            var nuevaReserva = new
            {
                EspacioId = 1,
                FechaInicio = DateTime.UtcNow.AddHours(1),
                FechaFin = DateTime.UtcNow.AddHours(2)
            };

            var response = await _client.PostAsJsonAsync("/api/reservas", nuevaReserva);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}




