using System.Net.Http.Headers;
using System.Text;
using NUnit.Framework;

namespace SmartPantry.WebApi.Tests.GeminiControllerTests.Base_Setup
{
    /// <summary>
    /// Base setup for GeminiController tests.
    /// Includes custom factory with IGeminiService mocked and
    /// JWT authentication using real login.
    /// </summary>
    public abstract class TestBase_GeminiControllerTests
    {
        protected CustomGeminiWebApplicationFactory _factory;
        protected HttpClient _client;

        [SetUp]
        public async Task BaseSetup()
        {
            _factory = new CustomGeminiWebApplicationFactory();
            _client = _factory.CreateClient();

            var token = await CreateAuthenticatedUserAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [TearDown]
        public void BaseTeardown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        /// <summary>
        /// Registers and logs in a test user, returning a valid JWT from the real pipeline.
        /// </summary>
        private async Task<string> CreateAuthenticatedUserAndGetTokenAsync()
        {
            var email = $"geminitest{Guid.NewGuid()}@example.com";

            // Register
            var registerPayload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"Password123\"}}",
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/user/register", registerPayload);

            // Login
            var loginPayload = new StringContent(
                $"{{\"email\":\"{email}\",\"password\":\"Password123\"}}",
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/user/login", loginPayload);
            var json = await response.Content.ReadAsStringAsync();

            var token = System.Text.Json.JsonDocument.Parse(json)
                .RootElement.GetProperty("token")
                .GetString();

            return token!;
        }
    }
}
