using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.DTOs.FoodProduct
{
    public class FoodProductAddDTO
    {
        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Quantity { get; set; }

        [MaxLength(100)]
        public string Brands { get; set; }

        [Required]
        [MaxLength(100)]
        public string Category { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }
    }
}
