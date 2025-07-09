namespace SmartPantry.Core.Entities
{
    public class FoodProductEntity
    {
        public Guid Id { get; set; }
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }
        public string Barcode { get; set; } 
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Brands { get; set; }
        public string Categories { get; set; }
        public DateTime AddedDate { get; set; }

        public ICollection<MealSuggestionFoodProductEntity> MealSuggestionFoodProducts { get; set; } = [];
    }
}
