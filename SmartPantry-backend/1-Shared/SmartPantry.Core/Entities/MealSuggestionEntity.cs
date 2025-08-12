using System;

namespace SmartPantry.Core.Entities
{
    public class MealSuggestionEntity
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }
        public string SuggestionText { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }

        public ICollection<MealSuggestionFoodProductEntity> MealSuggestionFoodProducts { get; set; } =
        [];
    }
}
