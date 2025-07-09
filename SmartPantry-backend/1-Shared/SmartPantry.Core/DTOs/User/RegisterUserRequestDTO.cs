using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.DTOs.User
{
    /// <summary>
    /// DTO for registering a user.
    /// Contains user input fields for registration.
    /// </summary>
    public class RegisterUserRequestDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
    }
}
