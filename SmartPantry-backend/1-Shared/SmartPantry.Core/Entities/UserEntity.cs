using System;

namespace SmartPantry.Core.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }  // Primary Key

        public string Email { get; set; }  // Should be unique

        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
