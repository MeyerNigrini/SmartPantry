using Microsoft.EntityFrameworkCore;
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

        public UserRepository(SmartPantryDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
