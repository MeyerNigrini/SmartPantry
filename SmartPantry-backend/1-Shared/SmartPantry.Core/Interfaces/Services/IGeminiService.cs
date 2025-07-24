namespace SmartPantry.Core.Interfaces.Services
{
    public interface IGeminiService
    {
        Task<string> GetGeminiResponse(string prompt);
        Task<List<string>> GetIngredientsFromFoodProducts(List<Guid> productIds);
    }
}
