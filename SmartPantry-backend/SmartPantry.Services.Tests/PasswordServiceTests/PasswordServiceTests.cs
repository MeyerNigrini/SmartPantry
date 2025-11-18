using AwesomeAssertions;
using NUnit.Framework;
using SmartPantry.Services.Services;

namespace SmartPantry.Services.Tests.PasswordServiceTests
{
    /// <summary>
    /// Tests for PasswordService covering hashing and verification behavior.
    /// </summary>
    [TestFixture]
    public class PasswordServiceTests
    {
        private PasswordService _service;

        [SetUp]
        public void Setup()
        {
            _service = new PasswordService();
        }

        // ---------------------------------------------------------------------
        // HASHPASSWORD
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies HashPassword returns a non-empty string.
        /// </summary>
        [Test]
        public void HashPassword_ValidPassword_ReturnsNonEmptyString()
        {
            // Arrange
            var password = "MySecret123";

            // Act
            var result = _service.HashPassword(password);

            // Assert
            result.Should().NotBeNullOrWhiteSpace();
        }

        /// <summary>
        /// Verifies HashPassword produces different hashes for the same password due to salting.
        /// </summary>
        [Test]
        public void HashPassword_SamePasswordTwice_ReturnsDifferentHashes()
        {
            // Arrange
            var password = "MySecret123";

            // Act
            var hash1 = _service.HashPassword(password);
            var hash2 = _service.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2);
        }

        /// <summary>
        /// Verifies HashPassword returns a string in format "salt:hash".
        /// </summary>
        [Test]
        public void HashPassword_ReturnsSaltAndHashSeparatedByColon()
        {
            // Arrange
            var password = "Test123!";

            // Act
            var hash = _service.HashPassword(password);

            // Assert
            var parts = hash.Split(':');
            parts.Length.Should().Be(2);
            parts[0].Should().NotBeNullOrWhiteSpace();
            parts[1].Should().NotBeNullOrWhiteSpace();
        }

        // ---------------------------------------------------------------------
        // VERIFYPASSWORD
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies VerifyPassword returns true for a correct password.
        /// </summary>
        [Test]
        public void VerifyPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "Correct123";
            var hash = _service.HashPassword(password);

            // Act
            var result = _service.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Verifies VerifyPassword returns false when password does not match the hash.
        /// </summary>
        [Test]
        public void VerifyPassword_IncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var correctPassword = "Correct123";
            var wrongPassword = "Wrong456";
            var hash = _service.HashPassword(correctPassword);

            // Act
            var result = _service.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Verifies VerifyPassword returns false when stored hash format is invalid.
        /// </summary>
        [Test]
        public void VerifyPassword_InvalidHashFormat_ReturnsFalse()
        {
            // Arrange
            var password = "anything";
            var invalidHash = "this-is-not-valid";

            // Act
            var result = _service.VerifyPassword(password, invalidHash);

            // Assert
            result.Should().BeFalse();
        }
    }
}
