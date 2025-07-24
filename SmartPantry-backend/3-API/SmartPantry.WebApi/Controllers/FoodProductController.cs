using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodProductController : ControllerBase
    {
        private readonly IFoodProductService _service;

        public FoodProductController(IFoodProductService service)
        {
            _service = service;
        }

        [HttpPost("addFoodProduct")]
        public async Task<IActionResult> AddFoodProduct([FromBody] FoodProductAddDTO dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            await _service.AddFoodProductAsync(dto, userId);
            return Ok(new { message = "Food product saved successfully" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodProductResponseDTO>>> GetAllFoodProducts()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var products = await _service.GetAllFoodProductsAsync(userId);
            return Ok(products);
        }
    }
}
