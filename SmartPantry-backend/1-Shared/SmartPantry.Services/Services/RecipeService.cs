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

        /// <inheritdoc />
        public async Task DeleteRecipeForUserAsync(Guid recipeId, Guid userId)
        {
            if (recipeId == Guid.Empty || userId == Guid.Empty)
                throw new InvalidInputException("Invalid recipe or user ID.");

            try
            {
                var deleted = await _repository.DeleteRecipeByIdForUserAsync(recipeId, userId);

                if (!deleted)
                    throw new InvalidInputException("Recipe not found or does not belong to the user.");
            }
            catch (PersistenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting recipe {RecipeId} for user {UserId}", recipeId, userId);
                throw new PersistenceException("Unexpected error occurred while deleting the recipe.", ex);
            }
        }

        /// <inheritdoc />
        public async Task<RecipeResponseDTO> UpdateRecipeForUserAsync(Guid recipeId, RecipeUpdateDTO dto, Guid userId)
        {
            if (recipeId == Guid.Empty || userId == Guid.Empty)
                throw new InvalidInputException("Invalid recipe or user ID.");

            if (dto == null)
                throw new InvalidInputException("Recipe update data is required.");

            // Ensure at least one field is being updated
            var hasUpdate =
                !string.IsNullOrWhiteSpace(dto.Title) ||
                (dto.Ingredients != null && dto.Ingredients.Any()) ||
                (dto.Instructions != null && dto.Instructions.Any());

            if (!hasUpdate)
                throw new InvalidInputException("At least one field must be provided to update the recipe.");

            try
            {
                // Only include fields that were actually provided
                var updatedEntity = new RecipeEntity
                {
                    Title = dto.Title?.Trim(),
                    Ingredients = dto.Ingredients != null
                        ? string.Join("\n", dto.Ingredients)
                        : null,
                    Instructions = dto.Instructions != null
                        ? string.Join("\n", dto.Instructions)
                        : null,
                    UpdatedAt = DateTime.UtcNow
                };

                var savedRecipe = await _repository.UpdateRecipeForUserAsync(recipeId, userId, updatedEntity);

                if (savedRecipe == null)
                    throw new InvalidInputException("Recipe not found or does not belong to the user.");

                return new RecipeResponseDTO
                {
                    Id = savedRecipe.Id,
                    Title = savedRecipe.Title,
                    Ingredients = (savedRecipe.Ingredients ?? string.Empty)
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                    Instructions = (savedRecipe.Instructions ?? string.Empty)
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .ToList(),
                    CreatedAt = savedRecipe.CreatedAt
                };
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recipe {RecipeId} for user {UserId}", recipeId, userId);
                throw new PersistenceException("Could not update the recipe.", ex);
            }
        }
    }
}
