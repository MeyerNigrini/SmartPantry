using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.DataAccess.Contexts;

namespace SmartPantry.DataAccess.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly SmartPantryDbContext _context;
        private readonly ILogger<RecipeRepository> _logger;

        public RecipeRepository(SmartPantryDbContext context, ILogger<RecipeRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddRecipeForUserAsync(RecipeEntity recipe)
        {
            if (recipe == null)
            {
                _logger.LogWarning("Attempted to add a null recipe entity.");
                return;
            }

            try
            {
                await _context.Recipes.AddAsync(recipe);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB update failed while adding recipe {RecipeId}", recipe.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding recipe {RecipeId}", recipe.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<List<RecipeEntity>> GetAllRecipesByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to query recipes with an empty UserId.");
                return new List<RecipeEntity>();
            }

            try
            {
                return await _context.Recipes
                    .Where(r => r.UserID == userId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recipes for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteRecipeByIdForUserAsync(Guid recipeId, Guid userId)
        {
            if (recipeId == Guid.Empty || userId == Guid.Empty)
            {
                _logger.LogWarning("Attempted to delete with invalid IDs. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                return false;
            }

            try
            {
                var recipe = await _context.Recipes
                    .FirstOrDefaultAsync(r => r.Id == recipeId && r.UserID == userId);

                if (recipe == null)
                {
                    _logger.LogInformation("No recipe found for delete. RecipeId: {RecipeId}, UserId: {UserId}", recipeId, userId);
                    return false;
                }

                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "DB update failed while deleting recipe {RecipeId} for user {UserId}", recipeId, userId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting recipe {RecipeId} for user {UserId}", recipeId, userId);
                throw;
            }
        }
    }
}
