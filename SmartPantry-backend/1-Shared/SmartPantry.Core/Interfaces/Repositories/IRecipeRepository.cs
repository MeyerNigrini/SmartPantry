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
    }
}
