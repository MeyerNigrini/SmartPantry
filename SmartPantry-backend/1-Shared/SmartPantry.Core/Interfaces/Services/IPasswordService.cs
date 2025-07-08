namespace SmartPantry.Core.Interfaces.Services
{
    /// <summary>
    /// Service interface for password hashing and verification.
    /// </summary>
    public interface IPasswordService
    {
        /// <summary>
        /// Hashes a plain text password securely using PBKDF2 with HMACSHA256.
        /// </summary>
        /// <param name="password">Plain text password to hash.</param>
        /// <returns>Hashed password including salt.</returns>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a plain text password against a previously hashed password.
        /// </summary>
        /// <param name="password">Plain text password to verify.</param>
        /// <param name="storedHash">Stored hash with salt to compare against.</param>
        /// <returns>True if password is valid, otherwise false.</returns>
        bool VerifyPassword(string password, string storedHash);
    }
}
