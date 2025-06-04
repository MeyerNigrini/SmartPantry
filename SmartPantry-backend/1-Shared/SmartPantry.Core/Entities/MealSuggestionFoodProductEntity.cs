using System;

namespace SmartPantry.Core.Entities
{
    public class MealSuggestionFoodProductEntity
    {
        public Guid MealSuggestionID { get; set; }
        public MealSuggestionEntity MealSuggestion { get; set; }
        public Guid FoodProductID { get; set; }
        public FoodProductEntity FoodProduct { get; set; }
        public int OrderIndex { get; set; }
    }
}
