using System.Net;
using System.Net.Http.Json;
using Xunit;
using Reserva.Api;

namespace Reserva.Tests.Api
{
    public class UsuarioControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public UsuarioControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_Usuario_And_Login_ReturnsToken()
        {
            var registerDto = new
            {
                Email = "nuevo@test.com",
                Password = "pass123"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

            var loginDto = new
            {
                Email = "nuevo@test.com",
                Password = "pass123"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<dynamic>();
            Assert.NotNull(loginContent.token);
        }
    }
}


