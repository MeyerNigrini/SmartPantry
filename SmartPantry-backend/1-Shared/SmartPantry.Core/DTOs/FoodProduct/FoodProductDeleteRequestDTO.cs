namespace SmartPantry.Core.DTOs.FoodProduct
{
    public class FoodProductDeleteRequestDTO
    {
        public List<Guid> ProductIds { get; set; } = new();
    }
}
