using SmartPantry.Core.DTOs.FoodProduct;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IFoodProductService
    {
        Task AddFoodProductAsync(FoodProductCreateDTO dto, Guid userId);
    }
}
