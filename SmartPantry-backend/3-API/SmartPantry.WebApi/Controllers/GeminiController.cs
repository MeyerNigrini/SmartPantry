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
                Using ONLY the ingredients listed below, generate a complete recipe in this EXACT JSON format:

                {
                  "title": "<generated title>",
                  "ingredients": ["ingredient 1", "ingredient 2", ...],
                  "instructions": ["Step 1", "Step 2", "Step 3", ...]
                }

                Do NOT change the JSON structure.

                Rules:          
                - Ingredients are given as: "<ProductName> - <Quantity> - <Brand> - <Category>"
                - When adding to the "ingredients" array:
                  • Keep only the product name and the useful cooking amount.
                  • Convert bulk amounts into realistic cooking measurements.
                    Examples:
                      "400 g" → "2 tbsp Peanut Butter" or "200 g Peanut Butter"
                      "500g" → treat as "500 g"
                      "1 L" → convert to "250 ml" or "1 cup"
                  • Sauces, spreads, condiments, and oils must be limited to small realistic amounts:
                      - Use 1–4 tbsp total unless the dish is sauce-based.
                      - Never convert these into cups unless absolutely necessary.
                  • Order ingredients in this sequence:
                      1. base items (wraps, rice, pasta)
                      2. proteins
                      3. vegetables and fruits
                      4. dairy and cheese
                      5. sauces, condiments, and oils
                      6. spices or extras
                  • Ingredient strings must look like normal cookbook ingredients.
                - Instructions must read like real cookbook steps:
                  • clear, short, professional
                  • reference only the selected ingredients
                  • avoid any commentary or reasoning
                - Title must sound like a proper cookbook recipe.
                  • Do NOT invent flavours or adjectives not clearly supported by the ingredients.
                  • Sweet Chili Sauce should NOT be treated as a spicy ingredient unless combined with hot spices.
                - Output must be valid JSON only (no markdown, no explanations, no text outside the braces).

                Ingredients:
                {{formattedIngredients}}
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
