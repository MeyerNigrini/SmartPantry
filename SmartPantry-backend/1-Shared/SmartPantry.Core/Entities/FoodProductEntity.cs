using System;

namespace SmartPantry.Core.Entities
{
    public class FoodProductEntity
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }
        public string Barcode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string Ingredients { get; set; } = null!;
        public string Quantity { get; set; } = null!;
        public DateTime? ExpiryDate { get; set; }
        public DateTime AddedDate { get; set; }

        public ICollection<MealSuggestionFoodProductEntity> MealSuggestionFoodProducts { get; set; } = [];
    }
}
