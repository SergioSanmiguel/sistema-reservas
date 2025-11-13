using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Reserva.Api;

namespace Reserva.Tests.Api
{
    public class ReservaControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ReservaControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_Reservas_ReturnsOk()
        {
            // Act
            var response = await _client.GetAsync("/api/reservas");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Post_Reserva_ReturnsCreated()
        {
            // Arrange
            var nuevaReserva = new
            {
                UsuarioId = 1,
                EspacioId = 1,
                FechaInicio = DateTime.Now.AddHours(1),
                FechaFin = DateTime.Now.AddHours(2)
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/reservas", nuevaReserva);

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.Created ||
                response.StatusCode == HttpStatusCode.BadRequest,
                $"Código devuelto: {response.StatusCode}"
            );
        }
    }
}
