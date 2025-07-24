using System.ComponentModel.DataAnnotations;

namespace SmartPantry.Core.DTOs.FoodProduct
{
    public class FoodProductAddDTO
    {
        [Required]
        public string Barcode { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public string Quantity { get; set; }
        [Required]
        public string Brands { get; set; }
        public string Categories { get; set; }
    }
}