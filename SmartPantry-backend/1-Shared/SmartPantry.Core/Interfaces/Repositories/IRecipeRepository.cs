using SmartPantry.Core.Entities;

namespace SmartPantry.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository interface for managing recipe persistence and retrieval operations.
    /// </summary>
    public interface IRecipeRepository
    {
        /// <summary>
        /// Adds a new recipe to the database for the specified user.
        /// </summary>
        /// <param name="recipe">The recipe entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRecipeForUserAsync(RecipeEntity recipe);

        /// <summary>
        /// Retrieves all recipes belonging to a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of recipe entities owned by the user.</returns>
        Task<List<RecipeEntity>> GetAllRecipesByUserIdAsync(Guid userId);

        /// <summary>
        /// Deletes the recipe by ID if it belongs to the given user.
        /// Returns true if deletion succeeded; false if not found or not owned by the user.
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to delete.</param>
        /// <param name="userId">The authenticated user's ID.</param>
        Task<bool> DeleteRecipeByIdForUserAsync(Guid recipeId, Guid userId);

        /// <summary>
        /// Updates an existing recipe owned by the specified user.
        /// Returns the updated recipe entity, or null if not found or not owned by the user.
        /// </summary>
        /// <param name="recipeId">The ID of the recipe to update.</param>
        /// <param name="userId">The authenticated user's ID.</param>
        /// <param name="updatedRecipe">The recipe entity containing updated values.</param>
        Task<RecipeEntity?> UpdateRecipeForUserAsync(Guid recipeId, Guid userId, RecipeEntity updatedRecipe);
    }
}
