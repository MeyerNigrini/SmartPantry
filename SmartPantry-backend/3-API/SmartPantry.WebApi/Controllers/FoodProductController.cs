using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodProductController : ControllerBase
    {
        private readonly IFoodProductService _service;
        private readonly ILogger<FoodProductController> _logger;

        public FoodProductController(IFoodProductService service, ILogger<FoodProductController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new food product for the authenticated user.
        /// </summary>
        [HttpPost("addFoodProductForUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFoodProductForUser([FromBody] FoodProductAddDTO dto)
        {
            // Extract the user ID from JWT claims. If invalid, reject the request.
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            try
            {
                await _service.AddFoodProductForUserAsync(dto, userId);
                return Ok(new { message = "Food product saved successfully" });
            }
            catch (InvalidInputException ex)
            {
                // Input was structurally valid but semantically wrong
                _logger.LogWarning(ex, "Invalid input while adding food product.");
                return BadRequest(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                // Service failed to persist data (repository issue, etc.)
                _logger.LogError(ex, "Database failure while adding food product.");
                return StatusCode(500, new { message = "Could not save food product." });
            }
            catch (Exception ex)
            {
                // Something unexpected — keep message generic for client, log details
                _logger.LogError(ex, "Unexpected error while adding food product.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Returns all food products belonging to the authenticated user.
        /// </summary>
        [HttpGet("getAllFoodProductsForUser")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<FoodProductResponseDTO>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<FoodProductResponseDTO>>> GetAllFoodProductsForUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            try
            {
                var products = await _service.GetAllFoodProductsForUserAsync(userId);
                return Ok(products);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID when fetching food products.");
                return BadRequest(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Data fetch failure in GetAllFoodProducts.");
                return StatusCode(500, new { message = "Failed to load food products." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetAllFoodProducts.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
