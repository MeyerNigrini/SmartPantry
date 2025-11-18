using AwesomeAssertions;
using Moq;
using NUnit.Framework;
using SmartPantry.Core.DTOs.User;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Services.Tests.UserServiceTests.Base_Setup;
using System.IdentityModel.Tokens.Jwt;

namespace SmartPantry.Services.Tests.UserServiceTests
{
    /// <summary>
    /// Tests for UserService ensuring correct registration and authentication logic.
    /// </summary>
    [TestFixture]
    public class UserServiceTests : TestBase_UserServiceTests
    {
        // ---------------------------------------------------------------------
        // REGISTER
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that a valid registration registers the user successfully.
        /// </summary>
        [Test]
        public async Task RegisterUserAsync_ValidRequest_RegistersUser()
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = " Test ",
                LastName = " User ",
                Email = " email@example.com ",
                Password = "password123"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("email@example.com"))
                         .ReturnsAsync((UserEntity)null);

            _passwordServiceMock.Setup(p => p.HashPassword("password123"))
                                .Returns("hashed");

            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<UserEntity>()))
                         .ReturnsAsync((UserEntity u) => u);

            // Act
            var result = await _service.RegisterUserAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("email@example.com");
            result.FirstName.Should().Be("Test");
            result.LastName.Should().Be("User");
        }

        /// <summary>
        /// Verifies trimming of input fields during registration.
        /// </summary>
        [Test]
        public async Task RegisterUserAsync_TrimmedInputs_AreTrimmed()
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = " John ",
                LastName = " Doe ",
                Email = " test@example.com ",
                Password = "123456"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync((UserEntity)null);

            _passwordServiceMock.Setup(p => p.HashPassword("123456"))
                                .Returns("hashed");

            UserEntity addedUser = null;

            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<UserEntity>()))
                         .Callback<UserEntity>(u => addedUser = u)
                         .Returns((UserEntity u) => Task.FromResult(u));

            // Act
            await _service.RegisterUserAsync(request);

            // Assert
            addedUser.FirstName.Should().Be("John");
            addedUser.LastName.Should().Be("Doe");
            addedUser.Email.Should().Be("test@example.com");
        }

        /// <summary>
        /// Verifies that missing email or password throws InvalidInputException.
        /// </summary>
        [TestCase("", "pass")]
        [TestCase(" ", "pass")]
        [TestCase("a@a.com", "")]
        [TestCase("a@a.com", " ")]
        public void RegisterUserAsync_MissingEmailOrPassword_ThrowsInvalidInputException(string email, string password)
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = "Test",
                LastName = "User",
                Email = email,
                Password = password
            };

            // Act + Assert
            Assert.ThrowsAsync<InvalidInputException>(async () =>
                await _service.RegisterUserAsync(request)
            );
        }

        /// <summary>
        /// Verifies that a duplicate email throws UserAlreadyExistsException.
        /// </summary>
        [Test]
        public void RegisterUserAsync_UserAlreadyExists_ThrowsUserAlreadyExistsException()
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = "A",
                LastName = "B",
                Email = "taken@example.com",
                Password = "123456"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("taken@example.com"))
                         .ReturnsAsync(new UserEntity());

            // Act + Assert
            Assert.ThrowsAsync<UserAlreadyExistsException>(async () =>
                await _service.RegisterUserAsync(request)
            );
        }

        /// <summary>
        /// Verifies that unexpected repository errors are wrapped into PersistenceException.
        /// </summary>
        [Test]
        public void RegisterUserAsync_RepositoryThrows_WrapsIntoPersistenceException()
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = "A",
                LastName = "B",
                Email = "test@example.com",
                Password = "123456"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ThrowsAsync(new Exception("DB is down"));

            // Act + Assert
            Assert.ThrowsAsync<PersistenceException>(async () =>
                await _service.RegisterUserAsync(request)
            );
        }

        /// <summary>
        /// Verifies that password hashing method is called during registration.
        /// </summary>
        [Test]
        public async Task RegisterUserAsync_PasswordIsHashed_UsesPasswordService()
        {
            // Arrange
            var request = new RegisterUserRequestDTO
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "password"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync((UserEntity)null);

            _passwordServiceMock.Setup(p => p.HashPassword("password"))
                                .Returns("hashedpw");

            _userRepoMock.Setup(r => r.AddUserAsync(It.IsAny<UserEntity>()))
                         .Returns((UserEntity u) => Task.FromResult(u));

            // Act
            await _service.RegisterUserAsync(request);

            // Assert
            _passwordServiceMock.Verify(p => p.HashPassword("password"), Times.Once);
        }

        // ---------------------------------------------------------------------
        // LOGIN
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that valid credentials return a JWT token.
        /// </summary>
        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginRequestDTO { Email = "test@example.com", Password = "pw" };

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashed"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(p => p.VerifyPassword("pw", "hashed"))
                                .Returns(true);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            result.Token.Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>
        /// Verifies that logging in with unknown email throws SmartPantryException.
        /// </summary>
        [Test]
        public void LoginAsync_UserNotFound_ThrowsSmartPantryException()
        {
            // Arrange
            var request = new LoginRequestDTO
            {
                Email = "missing@example.com",
                Password = "pw"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("missing@example.com"))
                         .ReturnsAsync((UserEntity)null);

            // Act + Assert
            Assert.ThrowsAsync<SmartPantryException>(async () =>
                await _service.LoginAsync(request)
            );
        }

        /// <summary>
        /// Verifies that an invalid password throws SmartPantryException.
        /// </summary>
        [Test]
        public void LoginAsync_InvalidPassword_ThrowsSmartPantryException()
        {
            // Arrange
            var request = new LoginRequestDTO { Email = "test@example.com", Password = "wrong" };

            var user = new UserEntity { Email = "test@example.com", PasswordHash = "hash" };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(p => p.VerifyPassword("wrong", "hash"))
                                .Returns(false);

            // Act + Assert
            Assert.ThrowsAsync<SmartPantryException>(async () =>
                await _service.LoginAsync(request)
            );
        }

        /// <summary>
        /// Verifies repository errors during login are wrapped into PersistenceException.
        /// </summary>
        [Test]
        public void LoginAsync_RepositoryThrows_WrapsIntoPersistenceException()
        {
            // Arrange
            var request = new LoginRequestDTO { Email = "test@example.com", Password = "pw" };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ThrowsAsync(new Exception("DB error"));

            // Act + Assert
            Assert.ThrowsAsync<PersistenceException>(async () =>
                await _service.LoginAsync(request)
            );
        }

        /// <summary>
        /// Verifies the returned JWT token has a valid structure.
        /// </summary>
        [Test]
        public async Task LoginAsync_ReturnedToken_HasValidJwtFormat()
        {
            // Arrange
            var request = new LoginRequestDTO { Email = "test@example.com", Password = "pw" };

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashed"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(p => p.VerifyPassword("pw", "hashed"))
                                .Returns(true);

            // Act
            var result = await _service.LoginAsync(request);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Token);

            token.Should().NotBeNull();
            token.Claims.Should().NotBeEmpty();
        }

        /// <summary>
        /// Verifies the returned token contains the expected claims.
        /// </summary>
        [Test]
        public async Task LoginAsync_ReturnedToken_HasCorrectClaims()
        {
            // Arrange
            var request = new LoginRequestDTO { Email = "test@example.com", Password = "pw" };

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "test@example.com",
                PasswordHash = "hash"
            };

            _userRepoMock.Setup(r => r.GetUserByEmailAsync("test@example.com"))
                         .ReturnsAsync(user);

            _passwordServiceMock.Setup(p => p.VerifyPassword("pw", "hash"))
                                .Returns(true);

            // Act
            var result = await _service.LoginAsync(request);
            var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Token);

            // Assert
            token.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id.ToString());
            token.Claims.Should().Contain(c => c.Type == "unique_name" && c.Value == user.FirstName);
            token.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        }
    }
}
