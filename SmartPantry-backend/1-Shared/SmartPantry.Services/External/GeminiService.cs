using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;

namespace SmartPantry.Services.External
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly IFoodProductRepository _foodProductRepository;
        private readonly string _geminiEndpoint;

        public GeminiService(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<GeminiService> logger,
            IFoodProductRepository foodProductRepository
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _foodProductRepository = foodProductRepository;

            var apiKey = config["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogCritical("Gemini API key is missing or empty.");
                throw new InvalidInputException("Gemini API key must be configured.");
            }

            _geminiEndpoint =
                $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
        }

        public async Task<List<string>> GetIngredientsFromFoodProducts(List<Guid> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                throw new InvalidInputException("Product ID list must not be empty.");

            try
            {
                var products = await _foodProductRepository.GetFoodProductsByIdsAsync(productIds);

                // Transform product data into prompt-friendly strings
                return products
                    .Select(p => $"{p.ProductName} - {p.Quantity} - {p.Brands} - {p.Categories}")
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve food products for Gemini prompt.");
                throw new PersistenceException("Could not retrieve food product data.", ex);
            }
        }

        public async Task<string> GetGeminiResponse(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_geminiEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError(
                        "Gemini API request failed: {StatusCode} - {Error}",
                        response.StatusCode,
                        error
                    );
                    throw new ExternalServiceException(
                        $"Gemini API error: {response.StatusCode}\n{error}"
                    );
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                try
                {
                    using var doc = JsonDocument.Parse(responseContent);
                    var rawText = doc
                        .RootElement.GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    // Remove markdown code blocks (``` markers)
                    rawText = rawText?.Trim();
                    if (rawText?.StartsWith("```") == true)
                    {
                        var lines = rawText.Split('\n');
                        rawText = string.Join(
                            "\n",
                            lines.Skip(1).TakeWhile(l => !l.StartsWith("```"))
                        );
                    }

                    return rawText ?? "[No content returned by Gemini]";
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Unexpected JSON structure in Gemini API response.");
                    throw new ExternalServiceException(
                        "Gemini returned an unexpected response format.",
                        parseEx
                    );
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Gemini API failed.");
                throw new ExternalServiceException("Unable to connect to Gemini API.", ex);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Gemini API request timed out.");
                throw new ExternalServiceException("Gemini API request timed out.", ex);
            }
            catch (ExternalServiceException)
            {
                throw; // Let wrapped, known exceptions bubble up
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error while calling Gemini API.");
                throw new ExternalServiceException(
                    "An unexpected error occurred while calling Gemini.",
                    ex
                );
            }
        }
    }
}
