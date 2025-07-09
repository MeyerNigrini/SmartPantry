using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.Interfaces.Services;

namespace YourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OpenFoodFactsController : ControllerBase
    {
        private readonly IOpenFoodFactsService _openFoodFactsService;

        public OpenFoodFactsController(IOpenFoodFactsService openFoodFactsService)
        {
            _openFoodFactsService = openFoodFactsService;
        }

        [HttpGet("{barcode}")]
        public async Task<IActionResult> GetProductDetails(string barcode)
        {
            var result = await _openFoodFactsService.GetProductDetailsByBarcodeAsync(barcode);

            if (result.StatusVerbose == "product not found")
            {
                return NotFound(new { Message = "Product not found" });
            }

            return Ok(result);
        }
    }
}
