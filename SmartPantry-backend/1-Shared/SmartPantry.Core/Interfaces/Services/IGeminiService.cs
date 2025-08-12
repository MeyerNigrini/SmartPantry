using SmartPantry.Core.DTOs.Gemini;

namespace SmartPantry.Core.Interfaces.Services
{
    public interface IGeminiService
    {
        Task<string> GetGeminiResponse(string prompt);
        Task<List<string>> GetIngredientsFromFoodProducts(List<Guid> productIds);
        Task<ProductVisionExtract> ExtractProductFromImageAsync(
            ImagePayload image,
            CancellationToken ct = default
        );
    }
}
