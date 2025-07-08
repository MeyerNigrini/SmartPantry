namespace SmartPantry.Core.Settings
{
    /// <summary>
    /// Strongly typed settings for JWT configuration, bound from appsettings.json.
    /// </summary>
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
