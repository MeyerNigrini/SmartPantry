using SmartPantry.Core.Entities;

namespace SmartPantry.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for user data access.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Adds a new user entity to the database asynchronously.
        /// </summary>
        /// <param name="user">User entity to add.</param>
        /// <returns>Added user entity with generated ID.</returns>
        Task<UserEntity> AddUserAsync(UserEntity user);

        /// <summary>
        /// Retrieves a user by email if they exist.
        /// </summary>
        /// <param name="email">Email address to search for.</param>
        /// <returns>User entity if found, otherwise null.</returns>
        Task<UserEntity?> GetUserByEmailAsync(string email);
    }
}
