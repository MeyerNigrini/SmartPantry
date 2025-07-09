using SmartPantry.Core.DTOs.OpenFoodFacts;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IOpenFoodFactsService
    {
        Task<ProductDetailsResponseDTO> GetProductDetailsByBarcodeAsync(string barcode);
    }
}
