using NUnit.Framework;
using System.Text;
using AwesomeAssertions;
using System.Net;
using SmartPantry.WebApi.Tests.UserControllerTests.Base_Setup;

namespace SmartPantry.WebApi.Tests.UserControllerTests
{
    /// <summary>
    /// Integration tests for UserController endpoints ensuring registration and login functionality.
    /// </summary>
    [TestFixture]
    public class UserControllerTests : TestBase_UserControllerTests
    {
        /// <summary>
        /// Tests that registering a new user returns HTTP 200 OK with valid response data.
        /// </summary>
        [Test]
        public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
        {
            var email = $"testregister{Guid.NewGuid()}@example.com";

            var payload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"password\"}}",
                Encoding.UTF8,
                "application/json"
            );

            var response = await _client.PostAsync("/api/user/register", payload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Tests that logging in with valid credentials returns HTTP 200 OK and includes a JWT token in the response.
        /// </summary>
        [Test]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // First, register the user
            var registerPayload = new StringContent(
                "{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"testlogin@example.com\",\"password\":\"password\"}",
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync("/api/user/register", registerPayload);

            // Then attempt login
            var loginPayload = new StringContent(
                "{\"email\":\"testlogin@example.com\",\"password\":\"password\"}",
                Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/user/login", loginPayload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var jsonString = await response.Content.ReadAsStringAsync();
            jsonString.Should().Contain("token");
        }

        /// <summary>
        /// Tests that logging in with invalid credentials returns HTTP 401 Unauthorized.
        /// </summary>
        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginPayload = new StringContent(
                "{\"email\":\"nonexisting@example.com\",\"password\":\"wrongpassword\"}",
                Encoding.UTF8,
                "application/json"
            );
            var response = await _client.PostAsync("/api/user/login", loginPayload);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
