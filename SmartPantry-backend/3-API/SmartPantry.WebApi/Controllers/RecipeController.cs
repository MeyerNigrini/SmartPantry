using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.Recipes;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _service;
        private readonly ILogger<RecipeController> _logger;

        public RecipeController(IRecipeService service, ILogger<RecipeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new recipe for the authenticated user.
        /// </summary>
        [HttpPost("addRecipeForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddRecipeForUser([FromBody] RecipeCreateDTO dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            try
            {
                await _service.AddRecipeForUserAsync(dto, userId);
                return Ok(new { message = "Recipe saved successfully" });
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid input while adding recipe for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Database failure while adding recipe for user {UserId}", userId);
                return StatusCode(500, new { message = "Could not save recipe." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding recipe for user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Retrieves all recipes belonging to the authenticated user.
        /// </summary>
        [HttpGet("getAllRecipesForUser")]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(IEnumerable<RecipeResponseDTO>)
        )]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RecipeResponseDTO>>> GetAllRecipesForUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            try
            {
                var recipes = await _service.GetAllRecipesForUserAsync(userId);
                return Ok(recipes);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID when fetching recipes for user {UserId}", userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Database failure while fetching recipes for user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to load recipes." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching recipes for user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Deletes a recipe owned by the authenticated user.
        /// </summary>
        [HttpDelete("deleteRecipeForUser/{recipeId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteRecipeForUser([FromRoute] Guid recipeId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            try
            {
                await _service.DeleteRecipeForUserAsync(recipeId, userId);
                return Ok(new { message = "Recipe deleted successfully" });
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid delete attempt for recipe {RecipeId} by user {UserId}", recipeId, userId);
                return NotFound(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Database failure while deleting recipe {RecipeId} for user {UserId}", recipeId, userId);
                return StatusCode(500, new { message = "Could not delete recipe." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting recipe {RecipeId} for user {UserId}", recipeId, userId);
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
