using System.Collections.Generic;

namespace SmartPantry.Core.DTOs
{
    public class RecipeResponseDTO
    {
        public string Title { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Instructions { get; set; }
    }
}
