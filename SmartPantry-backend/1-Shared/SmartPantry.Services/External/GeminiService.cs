using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Interfaces.Repositories;

namespace SmartPantry.Services.External
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly IFoodProductRepository _foodProductRepository;
        private readonly string _geminiEndpoint;


        public GeminiService(HttpClient httpClient, IConfiguration config, ILogger<GeminiService> logger, IFoodProductRepository foodProductRepository)
        {
            _httpClient = httpClient;
            _logger = logger;
            _foodProductRepository = foodProductRepository;

            var apiKey = config["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogError("Gemini API key is missing or empty in user secrets.");
                throw new ArgumentNullException(nameof(apiKey), "Gemini API key must be set in user secrets.");
            }

            _geminiEndpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
        }

        public async Task<List<string>> GetIngredientsFromFoodProducts(List<Guid> productIds)
        {
            try
            {
                var products = await _foodProductRepository.GetByIdsAsync(productIds);

                return products.Select(p =>
                    $"{p.ProductName} - {p.Quantity} - {p.Brands} - {p.Categories}"
                ).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch food products for recipe prompt.");
                throw;
            }
        }

        public async Task<string> GetGeminiResponse(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_geminiEndpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API request failed: {StatusCode} - {Error}", response.StatusCode, error);
                    throw new ApplicationException($"Gemini API error: {response.StatusCode}\n{error}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseContent);
                var rawText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                // Optional: clean Markdown code block if needed
                rawText = rawText?.Trim();
                if (rawText?.StartsWith("```") == true)
                {
                    var lines = rawText.Split('\n');
                    rawText = string.Join("\n", lines.Skip(1).TakeWhile(l => !l.StartsWith("```")));
                }

                return rawText!;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Gemini API failed.");
                throw;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Gemini API request timed out.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when calling Gemini API.");
                throw;
            }
        }
    }
}
