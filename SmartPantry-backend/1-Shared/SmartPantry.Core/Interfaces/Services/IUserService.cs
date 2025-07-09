using SmartPantry.Core.DTOs.User;

namespace SmartPantry.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for user authentication and management.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Authenticates a user based on provided credentials and generates a JWT token if successful.
        /// </summary>
        /// <param name="request">Login request DTO containing email and password.</param>
        /// <returns>LoginResponseDTO containing user information and JWT token.</returns>
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request);

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">Registration request DTO containing user details.</param>
        /// <returns>UserResponseDTO with created user information.</returns>
        Task<RegisterUserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request);
    }
}
