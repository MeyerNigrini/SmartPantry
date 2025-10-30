namespace SmartPantry.Core.Entities
{
    public class RecipeEntity
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }

        public string Title { get; set; } = null!;
        public string IngredientsJson { get; set; } = "[]";
        public string Instructions { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RecipeFoodProductEntity> RecipeFoodProducts { get; set; } = [];
    }
}
