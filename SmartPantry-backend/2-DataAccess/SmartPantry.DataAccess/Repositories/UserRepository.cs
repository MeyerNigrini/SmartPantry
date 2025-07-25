using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.DataAccess.Contexts;

namespace SmartPantry.DataAccess.Repositories
{
    /// <summary>
    /// Implementation of IUserRepository using Entity Framework Core.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SmartPantryDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(SmartPantryDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB update failed while adding user {Email}", user.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding user {Email}", user.Email);
                throw;
            }
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by email {Email}", email);
                throw;
            }
        }
    }
}
