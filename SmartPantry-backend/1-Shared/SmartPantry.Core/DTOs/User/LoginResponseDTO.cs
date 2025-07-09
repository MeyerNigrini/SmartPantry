namespace SmartPantry.Core.DTOs.User
{
    /// <summary>
    /// DTO for user login response containing JWT token.
    /// </summary>
    public class LoginResponseDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}
