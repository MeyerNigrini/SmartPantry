using SmartPantry.Core.Entities;

namespace SmartPantry.Core.Interfaces.Repositories
{
    public interface IFoodProductRepository
    {
        Task AddAsync(FoodProductEntity product);
        Task<List<FoodProductEntity>> GetByUserIdAsync(Guid userId);
    }
}
