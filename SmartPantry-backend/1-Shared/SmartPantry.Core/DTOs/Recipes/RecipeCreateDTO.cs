namespace SmartPantry.Core.DTOs.Recipes
{
    public class RecipeCreateDTO
    {
        public string Title { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
    }
}
