using Microsoft.Extensions.Logging;
using SmartPantry.Core.DTOs.FoodProduct;
using SmartPantry.Core.Entities;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Enums;

namespace SmartPantry.Services.Services
{
    public class FoodProductService : IFoodProductService
    {
        private readonly IFoodProductRepository _repository;
        private readonly ILogger<FoodProductService> _logger;
        private const int ExpiringThresholdDays = 7;

        public FoodProductService(
            IFoodProductRepository repository,
            ILogger<FoodProductService> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        // --- Status calculation helper ---
        private FoodProductStatus ComputeStatus(DateTime expirationDate)
        {
            var today = DateTime.UtcNow.Date;

            if (expirationDate < today)
                return FoodProductStatus.Expired;

            if ((expirationDate - today).TotalDays <= ExpiringThresholdDays)
                return FoodProductStatus.Expiring;

            return FoodProductStatus.Fresh;
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
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                Brands = dto.Brands,
                Category = dto.Category,
                ExpirationDate = dto.ExpirationDate.Date,
                AddedDate = DateTime.UtcNow,
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

        public async Task<IEnumerable<FoodProductResponseDTO>> GetAllFoodProductsForUserAsync(
            Guid userId
        )
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
                    Category = p.Category,
                    ExpirationDate = p.ExpirationDate,
                    Status = ComputeStatus(p.ExpirationDate).ToString(),
                    AddedDate = p.AddedDate,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving food products for user {UserId}", userId);
                throw new PersistenceException("Failed to retrieve food products.", ex);
            }
        }

        public async Task<int> DeleteFoodProductsForUserAsync(List<Guid> productIds, Guid userId)
        {
            if (userId == Guid.Empty)
                throw new InvalidInputException("User ID is invalid.");

            if (productIds == null || productIds.Count == 0)
                throw new InvalidInputException("At least one product ID is required.");

            try
            {
                var deleted = await _repository.DeleteFoodProductsByIdsAsync(userId, productIds);

                if (deleted == 0)
                {
                    // Surface a semantic error so the controller can send a 400
                    throw new InvalidInputException("No matching products found to delete.");
                }

                return deleted;
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting products {@ProductIds} for user {UserId}.",
                    productIds,
                    userId
                );
                // Keep external contract consistent with your other methods
                throw new PersistenceException("Failed to delete food products.", ex);
            }
        }
    }
}
