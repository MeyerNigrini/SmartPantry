using Microsoft.EntityFrameworkCore;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.DataAccess.Contexts;

namespace SmartPantry.DataAccess.Repositories
{
    public class FoodProductRepository : IFoodProductRepository
    {
        private readonly SmartPantryDbContext _context;

        public FoodProductRepository(SmartPantryDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(FoodProductEntity product)
        {
            await _context.FoodProducts.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FoodProductEntity>> GetByUserIdAsync(Guid userId)
        {
            return await _context.FoodProducts
                                 .Where(p => p.UserID == userId)
                                 .ToListAsync();
        }

        public async Task<List<FoodProductEntity>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.FoodProducts
                .Where(p => ids.Contains(p.Id))
                .ToListAsync();
        }

    }
}
