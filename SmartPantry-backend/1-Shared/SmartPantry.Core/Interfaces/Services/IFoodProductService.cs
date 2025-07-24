using SmartPantry.Core.DTOs.FoodProduct;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IFoodProductService
    {
        Task AddFoodProductAsync(FoodProductAddDTO dto, Guid userId);
        Task<IEnumerable<FoodProductResponseDTO>> GetAllFoodProductsAsync(Guid userId);
    }
}
