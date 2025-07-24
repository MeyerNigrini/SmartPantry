using System.Text.Json;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.DTOs.OpenFoodFacts;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.External
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenFoodFactsService> _logger;

        public OpenFoodFactsService(HttpClient httpClient, ILogger<OpenFoodFactsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductDetailsResponseDTO> GetProductDetailsByBarcodeAsync(string barcode)
        {
            var fallback = new ProductDetailsResponseDTO
            {
                Name = "Unknown",
                Brands = "Unknown",
                Categories = "Unknown",
                Quantity = "Unknown",
                StatusVerbose = "error"
            };

            try
            {
                var url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OpenFoodFacts request failed: {StatusCode}", response.StatusCode);
                    return fallback;
                }

                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                var statusVerbose = root.GetProperty("status_verbose").GetString();

                if (statusVerbose == "product not found")
                {
                    return new ProductDetailsResponseDTO
                    {
                        Name = "Unknown",
                        Brands = "Unknown",
                        Categories = "Unknown",
                        Quantity = "Unknown",
                        StatusVerbose = statusVerbose
                    };
                }

                if (!root.TryGetProperty("product", out var product))
                {
                    _logger.LogWarning("Missing 'product' property in OpenFoodFacts response for barcode {Barcode}", barcode);
                    return fallback;
                }

                string GetSafeString(JsonElement parent, string key)
                    => parent.TryGetProperty(key, out var prop) ? prop.GetString() ?? "Unknown" : "Unknown";

                return new ProductDetailsResponseDTO
                {
                    Name = GetSafeString(product, "product_name"),
                    Brands = GetSafeString(product, "brands"),
                    Categories = GetSafeString(product, "categories"),
                    Quantity = GetSafeString(product, "quantity"),
                    StatusVerbose = statusVerbose ?? "error"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse OpenFoodFacts response for barcode {Barcode}", barcode);
                return fallback;
            }
        }
    }
}
