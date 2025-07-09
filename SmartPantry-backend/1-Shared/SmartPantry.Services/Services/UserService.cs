using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartPantry.Core.DTOs.User;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Settings;

namespace SmartPantry.Services.Services
{
    /// <summary>
    /// Implementation of IUserService handling user registration, authentication, and JWT token generation.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserService> _logger;
        private readonly JWTSettings _jwtSettings;

        /// <summary>
        /// Constructs a new UserService with required dependencies.
        /// </summary>
        public UserService(
            IUserRepository userRepository, 
            IPasswordService passwordService, 
            ILogger<UserService> logger, 
            IOptions<JWTSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _logger = logger;
            _jwtSettings = jwtOptions.Value;
        }

        /// <inheritdoc/>
        public async Task<RegisterUserResponseDTO> RegisterUserAsync(RegisterUserRequestDTO request)
        {
            // Trimming of input fields
            request.FirstName = request.FirstName?.Trim();
            request.LastName = request.LastName?.Trim();
            request.Email = request.Email?.Trim();

            _logger.LogInformation("Attempting to register user with email {Email}", request.Email);

            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new UserAlreadyExistsException(request.Email);
            }

            var hashedPassword = _passwordService.HashPassword(request.Password);

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = hashedPassword,
                CreateDate = DateTime.UtcNow
            };

            await _userRepository.AddUserAsync(user);

            _logger.LogInformation("User with email {Email} registered successfully.", user.Email);

            return new RegisterUserResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }

        /// <inheritdoc/>
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            _logger.LogInformation("Attempting login for email {Email}", request.Email);

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid login attempt for email {Email}", request.Email);
                throw new SmartPantryException("Invalid email or password.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.FirstName),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.LogInformation("User {Email} logged in successfully.", user.Email);

            return new LoginResponseDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Token = tokenString
            };
        }
    }
}
