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

        [HttpPost]
        public async Task<IActionResult> CreateFoodProduct([FromBody] FoodProductCreateDTO dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            await _service.AddFoodProductAsync(dto, userId);
            return Ok(new { message = "Food product saved successfully" });
        }
    }
}
