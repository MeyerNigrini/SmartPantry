namespace SmartPantry.Core.DTOs.Recipes
{
    public class RecipeResponseDTO
    {
        /// Recipe ID.
        public Guid Id { get; set; }

        /// Recipe title.
        public string Title { get; set; }

        /// List of ingredients (split by newline from DB).
        public List<string> Ingredients { get; set; }

        /// Step-by-step instructions (split by newline from DB).
        public List<string> Instructions { get; set; }

        /// Date recipe was created.
        public DateTime CreatedAt { get; set; }
    }
}
