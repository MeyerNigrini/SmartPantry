using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartPantry.Core.Interfaces.Services;


namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeminiController : ControllerBase
    {
        private readonly IGeminiService _geminiService;

        public GeminiController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("recipes")]
        [Authorize]
        public async Task<IActionResult> Ask([FromBody] List<Guid> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return BadRequest("No ingredient IDs provided.");

            var ingredients = await _geminiService.GetIngredientsFromFoodProducts(selectedIds);

            if (!ingredients.Any())
                return NotFound("No matching food products found.");

            var formattedIngredients = string.Join("\n- ", ingredients);
            var prompt =
                "Using the ingredients listed below, generate a recipe in **this EXACT JSON format**:\n\n" +
                "{\n" +
                "  \"title\": \"<generated title>\",\n" +
                "  \"ingredients\": [\"ingredient 1\", \"ingredient 2\", ...],\n" +
                "  \"instructions\": [\"Step 1:\", \"Step 2:\", \"Step 3:\", ...]\n" +
                "}\n\n" +
                "Use only the ingredients provided.\n\n" +
                "Ingredients:\n- " + formattedIngredients;

            var recipe = await _geminiService.GetGeminiResponse(prompt);
            return Ok(recipe);
        }
    }
}
