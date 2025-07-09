using NUnit.Framework;
using SmartPantry.Core.DTOs.User;
using SmartPantry.Core.Entities;
using AwesomeAssertions;
using Moq;

namespace SmartPantry.Services.Tests.UserServiceTests
{
    /// <summary>
    /// Tests for UserService methods ensuring user registration and login logic correctness.
    /// </summary>
    [TestFixture]
    public class UserServiceTests : Base_Setup.TestBase_UserServiceTests
    {
        /// <summary>
        /// Verifies that RegisterUserAsync registers a user when the email does not already exist.
        /// </summary>
        [Test]
        public async Task RegisterUserAsync_ShouldRegister_WhenEmailDoesNotExist()
        {
            var request = new RegisterUserRequestDTO
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "password"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync((UserEntity)null);
            _passwordServiceMock.Setup(p => p.HashPassword(request.Password)).Returns("hashedpassword");
            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<UserEntity>())).ReturnsAsync((UserEntity u) => u);

            var result = await _service.RegisterUserAsync(request);

            result.Should().NotBeNull();
            result.Email.Should().Be("test@example.com");
        }

        /// <summary>
        /// Verifies that LoginAsync returns a JWT token when valid credentials are provided.
        /// </summary>
        [Test]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var request = new LoginRequestDTO
            {
                Email = "test@example.com",
                Password = "password"
            };

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync(request.Email)).ReturnsAsync(user);
            _passwordServiceMock.Setup(p => p.VerifyPassword(request.Password, user.PasswordHash)).Returns(true);

            var result = await _service.LoginAsync(request);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
        }
    }
}
