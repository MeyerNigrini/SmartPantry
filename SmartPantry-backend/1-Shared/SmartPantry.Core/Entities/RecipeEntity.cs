namespace SmartPantry.Core.Entities
{
    public class RecipeEntity
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }
        /// Recipe title.
        public string Title { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
