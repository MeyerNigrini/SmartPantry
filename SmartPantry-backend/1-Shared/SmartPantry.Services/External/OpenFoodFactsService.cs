using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.DTOs.OpenFoodFacts;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.External
{
    public class OpenFoodFactsService : IOpenFoodFactsService
    {
        private static readonly Regex DigitsOnly = new(@"^\d+$", RegexOptions.Compiled);
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenFoodFactsService> _logger;

        public OpenFoodFactsService(HttpClient httpClient, ILogger<OpenFoodFactsService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ProductDetailsResponseDTO> GetProductDetailsByBarcodeAsync(string barcode)
        {
            barcode = barcode?.Trim();

            if (string.IsNullOrWhiteSpace(barcode))
                throw new InvalidInputException("Barcode is required.");

            if (!DigitsOnly.IsMatch(barcode))
                throw new InvalidInputException("Barcode must contain only digits.");

            var url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("OpenFoodFacts returned 404 for barcode {Barcode}", barcode);
                    return NotFoundDto("product not found");
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OpenFoodFacts request failed: {StatusCode} for barcode {Barcode}", response.StatusCode, barcode);
                    throw new ExternalServiceException($"OpenFoodFacts request failed with status {response.StatusCode}.");
                }

                var jsonString = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                var statusVerbose = root.TryGetProperty("status_verbose", out var statusVerboseProp)
                    ? statusVerboseProp.GetString()
                    : "unknown";

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

                if (string.Equals(statusVerbose, "product not found", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("OpenFoodFacts returned 'product not found' for barcode {Barcode}", barcode);
                    return NotFoundDto(statusVerbose);
                }

                if (!root.TryGetProperty("product", out var product))
                {
                    _logger.LogWarning("Missing 'product' property in OpenFoodFacts response for barcode {Barcode}", barcode);
                    throw new ExternalServiceException("Malformed OpenFoodFacts response: 'product' missing.");
                }

                string GetSafeString(JsonElement parent, string key)
                    => parent.TryGetProperty(key, out var prop) ? prop.GetString() ?? "Unknown" : "Unknown";

                return new ProductDetailsResponseDTO
                {
                    Name = GetSafeString(product, "product_name"),
                    Brands = GetSafeString(product, "brands"),
                    Categories = GetSafeString(product, "categories"),
                    Quantity = GetSafeString(product, "quantity"),
                    StatusVerbose = statusVerbose ?? "ok"
                };
            }
            catch (InvalidInputException)
            {
                throw;
            }
            catch (ExternalServiceException)
            {
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse OpenFoodFacts response for barcode {Barcode}", barcode);
                throw new ExternalServiceException("Failed to parse OpenFoodFacts response.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error calling OpenFoodFacts for barcode {Barcode}", barcode);
                throw new ExternalServiceException("Network error while contacting OpenFoodFacts.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error calling OpenFoodFacts for barcode {Barcode}", barcode);
                throw new ExternalServiceException("Unexpected error while contacting OpenFoodFacts.", ex);
            }
        }
        private static ProductDetailsResponseDTO NotFoundDto(string statusVerbose) => new()
        {
            Name = "Unknown",
            Brands = "Unknown",
            Categories = "Unknown",
            Quantity = "Unknown",
            StatusVerbose = statusVerbose
        };
    }
}
