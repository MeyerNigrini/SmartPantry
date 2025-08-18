using SmartPantry.Core.DTOs.FoodProduct;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IFoodProductService
    {
        Task AddFoodProductForUserAsync(FoodProductAddDTO dto, Guid userId);
        Task<IEnumerable<FoodProductResponseDTO>> GetAllFoodProductsForUserAsync(Guid userId);
        Task<int> DeleteFoodProductsForUserAsync(List<Guid> productIds, Guid userId);

    }
}
