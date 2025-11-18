using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SmartPantry.WebApi.Tests.FoodProductControllerTests.Base_Setup
{
    /// <summary>
    /// Base setup for FoodProductController integration tests.
    /// Creates test user, logs in, and applies JWT to HttpClient.
    /// </summary>
    public abstract class TestBase_FoodProductControllerTests
    {
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient _client;

        [SetUp]
        public async Task BaseSetup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            var token = await CreateAuthenticatedUserAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        /// <summary>
        /// Registers + logs in a test user and returns JWT token from real pipeline.
        /// </summary>
        private async Task<string> CreateAuthenticatedUserAndGetTokenAsync()
        {
            var email = $"foodtest{Guid.NewGuid()}@example.com";

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

            return System.Text.Json.JsonDocument.Parse(json)
                .RootElement.GetProperty("token")
                .GetString()!;
        }
    }
}
