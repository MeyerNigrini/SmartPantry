using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartPantry.Core.DTOs.Gemini;
using SmartPantry.Core.Exceptions;
using SmartPantry.Core.Interfaces.Repositories;
using SmartPantry.Core.Interfaces.Services;
using SmartPantry.Core.Settings;

namespace SmartPantry.Services.External
{
    /// <summary>
    /// Gemini API integration for text and vision requests. Uses settings for non-secrets and pulls the API key from configuration (secrets.json).
    /// </summary>
    public class GeminiService : IGeminiService
    {
        private static readonly HashSet<string> AllowedImageMimeTypes = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "image/jpeg",
            "image/png",
            "image/webp",
            "image/heic",
            "image/heif",
        };

        private readonly HttpClient _httpClient;
        private readonly ILogger<GeminiService> _logger;
        private readonly IFoodProductRepository _foodProductRepository;
        private readonly GeminiSettings _settings;
        private readonly string _geminiEndpoint;

        public GeminiService(
            HttpClient httpClient,
            IConfiguration config,
            IOptions<GeminiSettings> settings,
            ILogger<GeminiService> logger,
            IFoodProductRepository foodProductRepository
        )
        {
            _httpClient = httpClient;
            _logger = logger;
            _foodProductRepository = foodProductRepository;
            _settings = settings.Value;

            var apiKey = config["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogCritical("Gemini API key is missing or empty.");
                throw new InvalidInputException("Gemini API key must be configured.");
            }

            if (_settings.TimeoutSeconds > 0)
                _httpClient.Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds);

            var model = string.IsNullOrWhiteSpace(_settings.Model)
                ? "gemini-2.5-flash"
                : _settings.Model;

            _geminiEndpoint =
                $"https://generativelanguage.googleapis.com/v1/models/{model}:generateContent?key={apiKey}";
        }

        /// <summary>
        /// Fetches product records and maps them to prompt-friendly strings.
        /// </summary>
        /// <param name="productIds">IDs to fetch.</param>
        /// <returns>List of strings describing products.</returns>
        /// <exception cref="InvalidInputException">If the list is null or empty.</exception>
        /// <exception cref="PersistenceException">If repository access fails.</exception>
        public async Task<List<string>> GetIngredientsFromFoodProducts(List<Guid> productIds)
        {
            if (productIds == null || productIds.Count == 0)
                throw new InvalidInputException("Product ID list must not be empty.");

            try
            {
                var products = await _foodProductRepository.GetFoodProductsByIdsAsync(productIds);
                return products
                    .Select(p => $"{p.ProductName} - {p.Quantity} - {p.Brands} - {p.Category}")
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve food products for Gemini prompt.");
                throw new PersistenceException("Could not retrieve food product data.", ex);
            }
        }

        /// <summary>
        /// Sends a plain-text prompt to Gemini and returns the extracted candidate text.
        /// </summary>
        /// <param name="prompt">User prompt.</param>
        /// <returns>Model text output.</returns>
        /// <exception cref="ExternalServiceException">For HTTP, timeout, or parsing issues.</exception>
        public async Task<string> GetGeminiResponse(string prompt)
        {
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
            };

            return await ExecuteWithGeminiHandling(
                async () =>
                {
                    var responseJson = await PostToGeminiAsync(requestBody, CancellationToken.None);
                    return ExtractCandidateText(responseJson);
                },
                "GetGeminiResponse"
            );
        }

        /// <summary>
        /// Sends an image to Gemini Vision and parses a structured product extraction.
        /// </summary>
        /// <param name="image">Image payload with bytes and MIME type.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Structured product data from the image.</returns>
        /// <exception cref="InvalidInputException">If the image fails basic validation.</exception>
        /// <exception cref="ExternalServiceException">For HTTP, timeout, or parsing issues.</exception>
        public async Task<ProductVisionExtract> ExtractProductFromImageAsync(
            ImagePayload image,
            string visionInstruction,
            CancellationToken ct = default
        )
        {
            ValidateImage(image, AllowedImageMimeTypes, _settings.MaxImageBytes);

            var base64 = Convert.ToBase64String(image.Bytes);

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new object[]
                        {
                            new { inline_data = new { mime_type = image.MimeType, data = base64 } },
                            new { text = visionInstruction },
                        },
                    },
                },
                generation_config = new
                {
                    temperature = 0.2,
                },
            };

            return await ExecuteWithGeminiHandling(
                async () =>
                {
                    var responseJson = await PostToGeminiAsync(requestBody, ct);
                    var text = ExtractCandidateText(responseJson);

                    var result = JsonSerializer.Deserialize<ProductVisionExtract>(
                        text,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (result == null)
                        throw new ExternalServiceException("Failed to parse Gemini JSON response.");

                    // Reject empty results
                    var isEmpty =
                        string.IsNullOrWhiteSpace(result.ProductName)
                        && string.IsNullOrWhiteSpace(result.Quantity)
                        && string.IsNullOrWhiteSpace(result.Brand)
                        && string.IsNullOrWhiteSpace(result.Category)
                        && string.IsNullOrWhiteSpace(result.ExpirationDate);

                    if (isEmpty)
                        throw new ExternalServiceException("AI did not detect any product details. Please retake the photo.");

                    return result;
                },
                "ExtractProductFromImageAsync"
            );
        }

        // --- Helpers ---

        /// <summary>
        /// Serializes the request, POSTs to Gemini, checks status, and returns the raw response JSON.
        /// </summary>
        /// <param name="body">Request body object.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Raw JSON response string.</returns>
        /// <exception cref="ExternalServiceException">For HTTP errors or timeouts.</exception>
        private async Task<string> PostToGeminiAsync(object body, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(body);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(_geminiEndpoint, content, ct);
                var responseContent = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "Gemini API error {Status}: {Body}",
                        response.StatusCode,
                        responseContent
                    );
                    throw new ExternalServiceException(
                        $"Gemini API error: {response.StatusCode}\n{responseContent}"
                    );
                }

                return responseContent;
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Gemini API request timed out.");
                throw new ExternalServiceException("Gemini API request timed out.", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request to Gemini API failed.");
                throw new ExternalServiceException("Unable to connect to Gemini API.", ex);
            }
        }

        /// <summary>
        /// Extracts the primary candidate text from Gemini's response JSON and strips code fences if present.
        /// </summary>
        /// <param name="responseJson">Raw JSON from Gemini.</param>
        /// <returns>Candidate text content.</returns>
        /// <exception cref="ExternalServiceException">If the content is empty or structure is unexpected.</exception>
        private static string ExtractCandidateText(string responseJson)
        {
            using var doc = JsonDocument.Parse(responseJson);
            var text = doc
                .RootElement.GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(text))
                throw new ExternalServiceException("Gemini returned empty content.");

            var trimmed = text.Trim();

            if (trimmed.StartsWith("```"))
            {
                var lines = trimmed.Split('\n');
                trimmed = string.Join("\n", lines.Skip(1).TakeWhile(l => !l.StartsWith("```")));
            }

            return trimmed;
        }

        /// <summary>
        /// Validates the image payload for presence, MIME type allowlist, and max byte size.
        /// </summary>
        /// <param name="image">Image payload.</param>
        /// <param name="allowedMime">Allowed MIME types.</param>
        /// <param name="maxBytes">Maximum allowed size in bytes.</param>
        /// <exception cref="InvalidInputException">If validation fails.</exception>
        private static void ValidateImage(
            ImagePayload image,
            ISet<string> allowedMime,
            int maxBytes
        )
        {
            if (image is null || image.Bytes is null || image.Bytes.Length == 0)
                throw new InvalidInputException("Image is required.");

            if (!allowedMime.Contains(image.MimeType))
                throw new InvalidInputException($"Unsupported image type: {image.MimeType}");

            if (image.Bytes.Length > maxBytes)
                throw new InvalidInputException($"Image is too large. Max {maxBytes} bytes.");
        }

        /// <summary>
        /// Runs an operation with consistent logging and wraps unexpected errors in <see cref="ExternalServiceException"/>.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="action">Async operation.</param>
        /// <param name="context">Log context label.</param>
        /// <returns>Result of the operation.</returns>
        /// <exception cref="ExternalServiceException">Re-thrown if already that type, or wrapped otherwise.</exception>
        private async Task<T> ExecuteWithGeminiHandling<T>(Func<Task<T>> action, string context)
        {
            try
            {
                return await action();
            }
            catch (ExternalServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in {Context}.", context);
                throw new ExternalServiceException(
                    "An unexpected error occurred while calling Gemini.",
                    ex
                );
            }
        }
    }
}
