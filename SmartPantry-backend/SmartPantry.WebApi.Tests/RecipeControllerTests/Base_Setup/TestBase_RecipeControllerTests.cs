using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace SmartPantry.WebApi.Tests.RecipeControllerTests.Base_Setup
{
    /// <summary>
    /// Base setup for RecipeController integration tests.
    /// Handles creating test users, logging in, and applying JWT auth headers.
    /// </summary>
    public abstract class TestBase_RecipeControllerTests
    {
        protected WebApplicationFactory<Program> _factory;
        protected HttpClient _client;

        [SetUp]
        public async Task BaseSetup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            // Automatically authenticate before each test
            var token = await CreateAuthenticatedUserAndGetTokenAsync();
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        [TearDown]
        public void BaseTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        /// <summary>
        /// Creates a unique test user, logs them in, and returns a valid JWT token.
        /// </summary>
        private async Task<string> CreateAuthenticatedUserAndGetTokenAsync()
        {
            var email = $"recipeuser{Guid.NewGuid()}@example.com";

            // Register user
            var registerPayload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"Password123\"}}",
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/user/register", registerPayload);

            // Login user
            var loginPayload = new StringContent(
                $"{{\"email\":\"{email}\",\"password\":\"Password123\"}}",
                Encoding.UTF8,
                "application/json");

            var loginResponse = await _client.PostAsync("/api/user/login", loginPayload);
            var json = await loginResponse.Content.ReadAsStringAsync();

            // Extract token
            var token = System.Text.Json.JsonDocument.Parse(json)
                .RootElement.GetProperty("token")
                .GetString();

            return token!;
        }
    }
}
