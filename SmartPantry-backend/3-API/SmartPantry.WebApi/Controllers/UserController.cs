using Microsoft.AspNetCore.Mvc;
using SmartPantry.Core.DTOs.User;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.WebApi.Controllers
{
    /// <summary>
    /// API Controller for user-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Constructs a new UserController with required dependencies.
        /// </summary>
        /// <param name="userService">Service handling user logic.</param>
        /// <param name="logger">Logger instance for this controller.</param>
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">User registration details.</param>
        /// <returns>Registered user data.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RegisterUserResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequestDTO request)
        {
            try
            {
                var result = await _userService.RegisterUserAsync(request);
                _logger.LogInformation("New user registered successfully: {Email}", result.Email);
                return Ok(result);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid input during user registration.");
                return BadRequest(new { message = ex.Message });
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogWarning(ex, "User registration failed, user already exists.");
                return Conflict(new { message = ex.Message });
            }
            catch (PersistenceException ex)
            {
                _logger.LogError(ex, "Database error during user registration.");
                return StatusCode(500, new { message = "Failed to register user." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user registration.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token upon successful login.
        /// </summary>
        /// <param name="request">User login credentials.</param>
        /// <returns>User details and JWT token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            try
            {
                var result = await _userService.LoginAsync(request);
                _logger.LogInformation("User {Email} logged in successfully.", result.Email);
                return Ok(result);
            }
            catch (InvalidInputException ex)
            {
                _logger.LogWarning(ex, "Invalid login input.");
                return BadRequest(new { message = ex.Message });
            }
            catch (SmartPantryException ex)
            {
                _logger.LogWarning(ex, "Login failed for email {Email}.", request.Email);
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user login.");
                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }
    }
}
