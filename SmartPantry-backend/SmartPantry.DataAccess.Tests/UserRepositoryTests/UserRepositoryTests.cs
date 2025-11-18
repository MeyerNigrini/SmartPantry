using AwesomeAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SmartPantry.Core.Entities;

namespace SmartPantry.DataAccess.Tests.UserRepositoryTests
{
    /// <summary>
    /// Tests for UserRepository methods ensuring correct database operations.
    /// </summary>
    [TestFixture]
    public class UserRepositoryTests : Base_Setup.TestBase_UserRepositoryTests
    {
        // ---------------------------------------------------------------------
        // AddUserAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that AddUserAsync successfully adds a user to the in-memory database.
        /// </summary>
        [Test]
        public async Task AddUserAsync_ValidUser_AddsUserToDatabase()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreateDate = DateTime.UtcNow,
            };

            // Act
            var result = await _userRepository.AddUserAsync(user);

            // Assert
            result.Should().NotBeNull();
            var users = await _context.Users.ToListAsync();
            users.Should().ContainSingle(u => u.Email == "test@example.com");
        }

        /// <summary>
        /// Verifies that AddUserAsync saves all user properties correctly.
        /// </summary>
        [Test]
        public async Task AddUserAsync_ValidUser_SavesAllPropertiesCorrectly()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PasswordHash = "abc123",
                CreateDate = DateTime.UtcNow
            };

            // Act
            var result = await _userRepository.AddUserAsync(user);

            // Assert
            var stored = await _context.Users.FirstAsync();
            stored.Id.Should().Be(user.Id);
            stored.FirstName.Should().Be("John");
            stored.LastName.Should().Be("Doe");
            stored.Email.Should().Be("john.doe@example.com");
            stored.PasswordHash.Should().Be("abc123");
        }

        /// <summary>
        /// Verifies that AddUserAsync allows duplicate emails since repository does not enforce uniqueness.
        /// </summary>
        [Test]
        public async Task AddUserAsync_DuplicateEmail_AddsBothRecords()
        {
            // Arrange
            var user1 = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "A",
                LastName = "User",
                Email = "duplicate@example.com",
                PasswordHash = "hash1",
                CreateDate = DateTime.UtcNow
            };

            var user2 = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "B",
                LastName = "User",
                Email = "duplicate@example.com",
                PasswordHash = "hash2",
                CreateDate = DateTime.UtcNow
            };

            // Act
            await _userRepository.AddUserAsync(user1);
            await _userRepository.AddUserAsync(user2);
            var results = await _context.Users.Where(x => x.Email == "duplicate@example.com").ToListAsync();

            // Assert
            results.Should().HaveCount(2);
        }

        // ---------------------------------------------------------------------
        // GetUserByEmailAsync
        // ---------------------------------------------------------------------

        /// <summary>
        /// Verifies that GetUserByEmailAsync returns the correct user when the email exists in the database.
        /// </summary>
        [Test]
        public async Task GetUserByEmailAsync_EmailExists_ReturnsCorrectUser()
        {
            // Arrange
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "existing@example.com",
                PasswordHash = "hash",
                CreateDate = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByEmailAsync("existing@example.com");

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("existing@example.com");
        }

        /// <summary>
        /// Verifies that GetUserByEmailAsync returns null when the user does not exist.
        /// </summary>
        [Test]
        public async Task GetUserByEmailAsync_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var missingEmail = "notfound@example.com";

            // Act
            var result = await _userRepository.GetUserByEmailAsync(missingEmail);

            // Assert
            result.Should().BeNull();
        }

        /// <summary>
        /// Verifies that GetUserByEmailAsync returns the first matching record when duplicates exist.
        /// </summary>
        [Test]
        public async Task GetUserByEmailAsync_MultipleUsersWithSameEmail_ReturnsFirstMatch()
        {
            // Arrange
            var user1 = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "First",
                LastName = "User",
                Email = "multi@example.com",
                PasswordHash = "h1",
                CreateDate = DateTime.UtcNow
            };

            var user2 = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Second",
                LastName = "User",
                Email = "multi@example.com",
                PasswordHash = "h2",
                CreateDate = DateTime.UtcNow
            };

            await _context.Users.AddRangeAsync(user1, user2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUserByEmailAsync("multi@example.com");

            // Assert
            result.Should().NotBeNull();
            result.FirstName.Should().Be("First");
        }
    }
}
