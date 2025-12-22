using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class ProductSaveViewModel
    {
        public int SellerId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required, Range(0.01, double.MaxValue), DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required, MaxLength(1000)]
        public string Details { get; set; } = null!;

        [Required, Range(1, 255)]
        public byte StockAmount { get; set; }

        public IList<IFormFile> Images { get; set; } = new List<IFormFile>();
        public List<string> ExistingImages { get; set; } = new();
    }
}
