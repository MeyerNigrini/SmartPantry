using System.Net;
using System.Text;
using AwesomeAssertions;
using NUnit.Framework;
using SmartPantry.WebApi.Tests.UserControllerTests.Base_Setup;

namespace SmartPantry.WebApi.Tests.UserControllerTests
{
    /// <summary>
    /// Full integration tests for UserController using the real pipeline,
    /// verifying registration and login behaviours.
    /// </summary>
    [TestFixture]
    public class UserControllerTests : TestBase_UserControllerTests
    {
        /// <summary>
        /// Registers a new user and verifies that the API returns HTTP 200 OK.
        /// </summary>
        [Test]
        public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var email = $"testregister{Guid.NewGuid()}@example.com";
            var payload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"password\"}}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/user/register", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        /// <summary>
        /// Attempts to register a user with an email that already exists.
        /// Expects HTTP 409 Conflict.
        /// </summary>
        [Test]
        public async Task Register_ShouldReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var email = $"conflicttest{Guid.NewGuid()}@example.com";
            var payload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"password\"}}",
                Encoding.UTF8,
                "application/json"
            );

            // First registration (should succeed)
            await _client.PostAsync("/api/user/register", payload);

            // Act - second registration attempt for same email
            var response = await _client.PostAsync("/api/user/register", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        /// <summary>
        /// Attempts to register a user with invalid input and expects HTTP 400 BadRequest.
        /// Input fails model validation.
        /// </summary>
        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange - missing password, invalid email, empty strings
            var payload = new StringContent(
                "{\"firstName\":\"\",\"lastName\":\"\",\"email\":\"not-an-email\"}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/user/register", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Logs in an existing user and expects HTTP 200 OK containing a JWT token.
        /// </summary>
        [Test]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange - register a new user
            var email = $"loginvalid{Guid.NewGuid()}@example.com";

            var registerPayload = new StringContent(
                $"{{\"firstName\":\"Test\",\"lastName\":\"User\",\"email\":\"{email}\",\"password\":\"password\"}}",
                Encoding.UTF8,
                "application/json"
            );
            await _client.PostAsync("/api/user/register", registerPayload);

            var loginPayload = new StringContent(
                $"{{\"email\":\"{email}\",\"password\":\"password\"}}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/user/login", loginPayload);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            body.Should().Contain("token");
        }

        /// <summary>
        /// Attempts login with incorrect credentials and expects HTTP 401 Unauthorized.
        /// </summary>
        [Test]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginPayload = new StringContent(
                "{\"email\":\"unknown@example.com\",\"password\":\"wrongpassword\"}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/user/login", loginPayload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Attempts login with invalid request input (fails model validation).
        /// Expects HTTP 400 BadRequest.
        /// </summary>
        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange - missing fields
            var payload = new StringContent(
                "{\"email\":\"not-an-email\"}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/user/login", payload);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
