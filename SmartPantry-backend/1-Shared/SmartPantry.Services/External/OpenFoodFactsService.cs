using System.Text.Json;
using SmartPantry.Core.DTOs.OpenFoodFacts;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.External
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        private readonly HttpClient _httpClient;

        public OpenFoodFactsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDetailsResponseDTO> GetProductDetailsByBarcodeAsync(string barcode)
        {
            var url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch product details from OpenFoodFacts API.");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            var statusVerbose = root.GetProperty("status_verbose").GetString();

            if (statusVerbose == "product not found")
            {
                return new ProductDetailsResponseDTO
                {
                    StatusVerbose = "product not found"
                };
            }

            var product = root.GetProperty("product");
            var name = product.GetProperty("product_name").GetString() ?? "Unknown";
            var quantity = product.GetProperty("quantity").GetString() ?? "Unknown";
            var brands = product.TryGetProperty("brands", out var brandsProp) ? brandsProp.GetString() ?? "Unknown" : "Unknown";
            var categories = product.TryGetProperty("categories", out var categoriesProp) ? categoriesProp.GetString() ?? "Unknown" : "Unknown";

            return new ProductDetailsResponseDTO
            {
                Name = name,
                Brands = brands,
                Categories = categories,
                Quantity = quantity,
                StatusVerbose = statusVerbose ?? string.Empty
            };
        }
    }
}
