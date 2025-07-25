using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.OpenFoodFacts;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace YourApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OpenFoodFactsController : ControllerBase
    {
        private readonly IOpenFoodFactsService _openFoodFactsService;
        private readonly ILogger<OpenFoodFactsController> _logger;

        public OpenFoodFactsController(IOpenFoodFactsService openFoodFactsService, ILogger<OpenFoodFactsController> logger)
        {
            _openFoodFactsService = openFoodFactsService;
            _logger = logger;
        }

        [HttpGet("{barcode}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductDetailsResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status502BadGateway)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductDetails(string barcode)
        {
            try
            {
                var result = await _openFoodFactsService.GetProductDetailsByBarcodeAsync(barcode);

                if (string.Equals(result.StatusVerbose, "product not found", StringComparison.OrdinalIgnoreCase))
                    return NotFound(new { message = "Product not found" });

                return Ok(result);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid barcode input: {Barcode}", barcode);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "External service (OpenFoodFacts) error for barcode {Barcode}", barcode);
                return StatusCode(StatusCodes.Status502BadGateway,
                    new { message = "External service is currently unavailable. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching product {Barcode}", barcode);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An unexpected error occurred." });
            }
        }
    }
}
