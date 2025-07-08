namespace SmartPantry.Core.Entities
{
    /// <summary>
    /// Represents a user in the system.
    /// Stores user details, password hash, and related entities.
    /// </summary>
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreateDate { get; set; }

        public ICollection<FoodProductEntity> FoodProducts { get; set; } = [];
        public ICollection<MealSuggestionEntity> MealSuggestions { get; set; } = [];
    }
}
