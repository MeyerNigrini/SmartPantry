using SmartPantry.Core.Entities;

namespace SmartPantry.Core.Interfaces.Repositories
{
    public interface IFoodProductRepository
    {
        Task AddFoodProductForUserAsync(FoodProductEntity product);
        Task<List<FoodProductEntity>> GetAllFoodProductsByUserIdAsync(Guid userId);
        Task<List<FoodProductEntity>> GetFoodProductsByIdsAsync(List<Guid> ids);

    }
}
