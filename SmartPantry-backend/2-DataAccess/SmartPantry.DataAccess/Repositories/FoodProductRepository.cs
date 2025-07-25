using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.DataAccess.Contexts;

namespace SmartPantry.DataAccess.Repositories
{
    public class FoodProductRepository : IFoodProductRepository
    {
        private readonly SmartPantryDbContext _context;
        private readonly ILogger<FoodProductRepository> _logger;

        public FoodProductRepository(SmartPantryDbContext context, ILogger<FoodProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddFoodProductForUserAsync(FoodProductEntity product)
        {
            try
            {
                await _context.FoodProducts.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB update failed while adding food product {ProductId}", product.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddAsync for product {ProductId}", product.Id);
                throw;
            }
        }

        public async Task<List<FoodProductEntity>> GetAllFoodProductsByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to query food products with an empty UserId.");
                return new List<FoodProductEntity>();
            }

            try
            {
                return await _context.FoodProducts
                                     .Where(p => p.UserID == userId)
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving food products for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<FoodProductEntity>> GetFoodProductsByIdsAsync(List<Guid> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                _logger.LogWarning("GetByIdsAsync was called with an empty or null list.");
                return new List<FoodProductEntity>();
            }

            try
            {
                return await _context.FoodProducts
                                     .Where(p => ids.Contains(p.Id))
                                     .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving food products for IDs: {Ids}", string.Join(", ", ids));
                throw;
            }
        }

    }
}
