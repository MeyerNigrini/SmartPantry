using Microsoft.Extensions.Logging;
using SmartPantry.Core.DTOs.Recipes;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _repository;
        private readonly ILogger<RecipeService> _logger;

        public RecipeService(IRecipeRepository repository, ILogger<RecipeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddRecipeForUserAsync(RecipeCreateDTO dto, Guid userId)
        {
            if (dto == null)
                throw new InvalidInputException("Recipe data is required.");

            if (userId == Guid.Empty)
                throw new InvalidInputException("User ID is invalid.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new InvalidInputException("Recipe title is required.");

            try
            {
                var recipe = new RecipeEntity
                {
                    Id = Guid.NewGuid(),
                    UserID = userId,
                    Title = dto.Title.Trim(),
                    Ingredients = string.Join("\n", dto.Ingredients ?? new List<string>()),
                    Instructions = string.Join("\n", dto.Instructions ?? new List<string>()),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };

                await _repository.AddRecipeForUserAsync(recipe);
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding recipe for user {UserId}", userId);
                throw new PersistenceException("Could not save the recipe.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RecipeResponseDTO>> GetAllRecipesForUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidInputException("User ID is invalid.");

            try
            {
                var recipes = await _repository.GetAllRecipesByUserIdAsync(userId);

                return recipes.Select(r => new RecipeResponseDTO
                {
                    Id = r.Id,
                    Title = r.Title,
                    Ingredients = (r.Ingredients ?? string.Empty)
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                    Instructions = (r.Instructions ?? string.Empty)
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                    CreatedAt = r.CreatedAt,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recipes for user {UserId}", userId);
                throw new PersistenceException("Failed to retrieve recipes.", ex);
            }
        }
    }
}
