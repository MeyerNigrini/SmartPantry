using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.DTOs.User
{
    /// <summary>
    /// DTO for user login input.
    /// </summary>
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
