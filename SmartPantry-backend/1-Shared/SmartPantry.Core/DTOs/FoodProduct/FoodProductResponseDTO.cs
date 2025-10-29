namespace SmartPantry.Core.DTOs.FoodProduct
{
    public class FoodProductResponseDTO
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string Brands { get; set; }
        public string Category { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Status { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
