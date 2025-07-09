using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.Services
{
    public class FoodProductService : IFoodProductService
    {
        private readonly IFoodProductRepository _repository;

        public FoodProductService(IFoodProductRepository repository)
        {
            _repository = repository;
        }

        public async Task AddFoodProductAsync(FoodProductCreateDTO dto, Guid userId)
        {
            var entity = new FoodProductEntity
            {
                Id = Guid.NewGuid(),
                UserID = userId,
                Barcode = dto.Barcode,
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                Brands = dto.Brands,
                Categories = dto.Categories,
                AddedDate = DateTime.UtcNow
            };

            await _repository.AddAsync(entity);
        }
    }
}
