using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.Entities
{
    public class FoodProductEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public Guid UserID { get; set; }
        public UserEntity User { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Quantity { get; set; }

        [MaxLength(100)]
        public string Brands { get; set; }

        [MaxLength(100)]
        public string Category { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
