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
        /// <summary>
        /// Verifies that AddUserAsync successfully adds a user to the in-memory database.
        /// </summary>
        [Test]
        public async Task AddUserAsync_ShouldAddUserToDatabase()
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                CreateDate = DateTime.UtcNow,
            };

            var result = await _userRepository.AddUserAsync(user);

            result.Should().NotBeNull();
            var users = await _context.Users.ToListAsync();
            users.Should().ContainSingle(u => u.Email == "test@example.com");
        }

        /// <summary>
        /// Verifies that GetUserByEmailAsync returns the correct user when the email exists in the database.
        /// </summary>
        [Test]
        public async Task GetUserByEmailAsync_ShouldReturnCorrectUser_WhenExists()
        {
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

            var result = await _userRepository.GetUserByEmailAsync("existing@example.com");

            result.Should().NotBeNull();
            result.Email.Should().Be("existing@example.com");
        }
    }
}
