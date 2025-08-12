using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;
        private readonly ILogger<GeminiController> _logger;

        public GeminiController(IGeminiService geminiService, ILogger<GeminiController> logger)
        {
            _geminiService = geminiService;
            _logger = logger;
        }

        /// <summary>
        /// Generates a recipe based on the authenticated user's selected food product IDs.
        /// </summary>
        /// <param name="selectedIds">The GUIDs of food products to include in the recipe prompt.</param>
        /// <returns>A generated recipe in JSON format.</returns>
        [HttpPost("generate-recipe")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateRecipeFromSelectedProducts(
            [FromBody] List<Guid> selectedIds
        )
        {
            try
            {
                if (selectedIds == null || !selectedIds.Any())
                    return BadRequest("No ingredient IDs provided.");

                var ingredients = await _geminiService.GetIngredientsFromFoodProducts(selectedIds);

                if (!ingredients.Any())
                    return NotFound("No matching food products found.");

                var formattedIngredients = string.Join("\n- ", ingredients);

                var prompt =
                    "Using the ingredients listed below, generate a recipe in **this EXACT JSON format**:\n\n"
                    + "{\n"
                    + "  \"title\": \"<generated title>\",\n"
                    + "  \"ingredients\": [\"ingredient 1\", \"ingredient 2\", ...],\n"
                    + "  \"instructions\": [\"Step 1:\", \"Step 2:\", \"Step 3:\", ...]\n"
                    + "}\n\n"
                    + "Use only the ingredients provided.\n\n"
                    + "Ingredients:\n- "
                    + formattedIngredients;

                var recipe = await _geminiService.GetGeminiResponse(prompt);

                return Ok(recipe);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid input in GenerateRecipeFromSelectedProducts.");
                return BadRequest(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Database error in GenerateRecipeFromSelectedProducts.");
                return StatusCode(500, new { message = "Failed to fetch food products." });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Gemini API failed in GenerateRecipeFromSelectedProducts.");
                return StatusCode(
                    502,
                    new { message = "AI service is currently unavailable. Please try again later." }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GenerateRecipeFromSelectedProducts.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
