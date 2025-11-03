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
        /// Updates an existing recipe that belongs to the specified user.
        /// </summary>
        /// <param name="dto">The updated recipe details, including title, ingredients, and instructions.</param>
        /// <param name="userId">The ID of the user who owns the recipe.</param>
        /// <exception cref="InvalidInputException">
        /// Thrown when the input data or user ID is invalid, or when the recipe does not exist for the user.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// Thrown when a database or repository failure occurs during update.
        /// </exception>
    }
}
