namespace SmartPantry.Core.Settings
{
    public class GeminiSettings
    {
        public string Model { get; set; } = "gemini-2.0-flash-001";
        public int TimeoutSeconds { get; set; } = 60;
        public int MaxImageBytes { get; set; } = 20 * 1024 * 1024;
    }
}
