namespace SmartPantry.Core.DTOs.Recipes
{
    public class RecipeUpdateDTO
    {
        public string? Title { get; set; }
        public List<string>? Ingredients { get; set; }
        public List<string>? Instructions { get; set; }
    }
}
