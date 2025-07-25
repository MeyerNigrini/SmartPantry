using Microsoft.Extensions.Logging;
using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.Services
{
    public class FoodProductService : IFoodProductService
    {
        private readonly IFoodProductRepository _repository;
        private readonly ILogger<FoodProductService> _logger;

        public FoodProductService(IFoodProductRepository repository, ILogger<FoodProductService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task AddFoodProductForUserAsync(FoodProductAddDTO dto, Guid userId)
        {
            if (dto == null)
                throw new InvalidInputException("Food product data is required.");

            if (userId == Guid.Empty)
                throw new InvalidInputException("User ID is invalid.");


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
            
            try
            {
                await _repository.AddFoodProductForUserAsync(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding food product for user {UserId}", userId);
                throw new ApplicationException("Could not save the food product.");
            }
        }

        public async Task<IEnumerable<FoodProductResponseDTO>> GetAllFoodProductsForUserAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidInputException("User ID is invalid.");

            try
            {
                var entities = await _repository.GetAllFoodProductsByUserIdAsync(userId);

                return entities.Select(p => new FoodProductResponseDTO
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Quantity = p.Quantity,
                    Brands = p.Brands,
                    Categories = p.Categories,
                    AddedDate = p.AddedDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving food products for user {UserId}", userId);
                throw new PersistenceException("Failed to retrieve food products.", ex);
            }
        }
    }
}
