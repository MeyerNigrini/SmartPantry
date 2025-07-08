namespace SmartPantry.Core.DTOs
{
    /// <summary>
    /// DTO representing user data returned after registration.
    /// Excludes sensitive fields like password hash.
    /// </summary>
    public class UserResponseDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
