using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SmartPantry.Core.Interfaces.Services;
using System.Text.Json;
using SmartPantry.Core.DTOs;

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

        [HttpPost]
        public async Task<IActionResult> Ask()
        {
            // Replace with actual data from DB
            string[] ingredients = new[]
            {
                "Rice noodles",
                "Peanut",
                "Soy protein",
                "Green onions",
                "Chickpeas",
                "Sesame oil",
                "Chili",
                "Cumin",
                "Clove",
                "Citric acid"
            };

            var formattedIngredients = string.Join("\n- ", ingredients);

            var prompt =
                "Using the following ingredients, generate a recipe in **this EXACT JSON format**:\n\n" +
                "{\n" +
                "  \"title\": \"<generated title>\",\n" +
                "  \"ingredients\": [\"ingredient 1\", \"ingredient 2\", ...],\n" +
                "  \"instructions\": [\"Step 1:\", \"Step 2:\", \"Step 3:\", ...]\n" +
                "}\n\n" +
                "Ingredients:\n- " + formattedIngredients;

            var recipe = await _geminiService.GetGeminiResponse(prompt);
            return Ok(recipe);
        }
    }
}
