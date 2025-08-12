namespace SmartPantry.Core.DTOs.Gemini
{
    public record ProductVisionExtract(
        string ProductName,
        string? Quantity,
        string? Brand,
        List<string> Categories
    );
}
