using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.DTOs.FoodProduct
{
    public class FoodProductAddDTO
    {
        [MaxLength(100)]
        public string Barcode { get; set; }

        [Required]
        [MaxLength(255)]
        public string ProductName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Quantity { get; set; }

        [MaxLength(100)]
        public string Brands { get; set; }

        [MaxLength(100)]
        public string Categories { get; set; }
        [Required]
        public DateTime ExpirationDate { get; set; }
    }
}
