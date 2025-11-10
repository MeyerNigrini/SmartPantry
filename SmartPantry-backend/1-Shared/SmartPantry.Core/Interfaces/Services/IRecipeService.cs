using SmartPantry.Core.DTOs.Recipes;

namespace SmartPantry.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for handling recipe creation, mapping, and retrieval logic.
    /// </summary>
    public interface IRecipeService
    {
        /// <summary>
        /// Creates and saves a new recipe for the given user.
        /// </summary>
        /// <param name="dto">The DTO containing recipe details such as title, ingredients, and linked food products.</param>
        /// <param name="userId">The unique identifier of the user creating the recipe.</param>
        /// <returns>A task representing the asynchronous save operation.</returns>
        Task AddRecipeForUserAsync(RecipeCreateDTO dto, Guid userId);

        /// <summary>
        /// Retrieves all recipes created or saved by the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose recipes are requested.</param>
        /// <returns>A collection of recipe response DTOs.</returns>
        Task<IEnumerable<RecipeResponseDTO>> GetAllRecipesForUserAsync(Guid userId);

        /// <summary>
        /// Deletes a recipe if it belongs to the given user.
        /// </summary>
        /// <param name="recipeId">The recipe ID to delete.</param>
        /// <param name="userId">The authenticated user's ID.</param>
        /// <exception cref="InvalidInputException">
        /// Thrown when IDs are invalid or the recipe does not exist / is not owned by the user.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// Thrown when a repository/database failure occurs.
        /// </exception>
        Task DeleteRecipeForUserAsync(Guid recipeId, Guid userId);

    }
}
