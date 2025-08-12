namespace SmartPantry.Core.Settings
{
    public class GeminiSettings
    {
        public string Model { get; set; } = "gemini-2.5-flash";
        public int TimeoutSeconds { get; set; } = 20;
        public int MaxImageBytes { get; set; } = 20 * 1024 * 1024;
    }
}
