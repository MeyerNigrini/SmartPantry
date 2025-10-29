using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.Gemini;
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

                var prompt = $$"""
                Using the ingredients listed below, generate a recipe in **this EXACT JSON format**:

                {
                  "title": "<generated title>",
                  "ingredients": ["ingredient 1", "ingredient 2", ...],
                  "instructions": ["Step 1:", "Step 2:", "Step 3:", ...]
                }

                Rules:
                - Use only the ingredients provided.
                - Write clear, concise instructions (3–6 steps maximum).
                - Avoid adding extra ingredients not in the list.
                - Format must be valid JSON (no markdown, no text outside braces).
                - Do not include reasoning or commentary.

                Ingredients:
                - {{formattedIngredients}}
                """;

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

        /// <summary>
        /// Extracts product metadata (name, quantity, brand, categories) from a product photo.
        /// </summary>
        [HttpPost("extract-product")]
        [Consumes("multipart/form-data")]
        [RequestFormLimits(MultipartBodyLengthLimit = 20 * 1024 * 1024)]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [ProducesResponseType(typeof(ProductVisionExtract), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExtractProduct(IFormFile image, CancellationToken ct)
        {
            try
            {
                if (image is null || image.Length == 0)
                    return BadRequest(new { message = "Image file is required." });

                var categories = new[]
                {
                    "Fruits","Vegetables","Meat & Poultry","Seafood","Dairy & Eggs",
                    "Grains","Legumes","Nuts & Seeds","Bakery","Snacks","Beverages",
                    "Alcohol","Condiments","Spices & Herbs","Frozen","Prepared Meals",
                    "Canned Foods","Oils & Fats"
                };
                var categoryList = string.Join("\", \"", categories);

                var visionInstruction = $$"""
                Return ONLY valid JSON (no markdown, no explanations) in this structure:
                {
                  "ProductName": string,
                  "Quantity": string | null,
                  "Brand": string | null,
                  "Category": string,
                  "ExpirationDate": string | null
                }

                Rules:
                - The Category MUST be one of: ["{{categoryList}}"]
                - Always return one category from the list, never null.
                - If unsure, pick the closest match.
                - For ExpirationDate:
                  • If visible, format must be strictly "YYYY-MM-DD".
                  • If none visible, return null.
                - Do not include explanations, reasoning, or markdown — JSON only.
                """;

                await using var ms = new MemoryStream();
                await image.CopyToAsync(ms, ct);

                var payload = new ImagePayload(ms.ToArray(), image.ContentType);
                var result = await _geminiService.ExtractProductFromImageAsync(payload, visionInstruction, ct);

                return Ok(result);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid image input in ExtractProduct.");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Gemini API failed in ExtractProduct.");
                return StatusCode(
                    502,
                    new { message = ex.Message }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ExtractProduct.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
