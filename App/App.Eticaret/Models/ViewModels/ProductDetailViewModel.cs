namespace App.Eticaret.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Details { get; set; } = null!;
        public string[] ImageUrls { get; set; } = [];
    }
}
