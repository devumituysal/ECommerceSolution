namespace App.Eticaret.Models.ViewModels
{
    public class ProductDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int SellerId { get; set; }
        public string SellerName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price { get; set; }
        public string Details { get; set; } = null!;
        public string[] ImageUrls { get; set; } = [];
        public int StockAmount { get; set; }
        public bool IsFavorite { get; set; }
        public List<ProductCommentViewModel> Comments { get; set; } = new();
        public SaveProductCommentViewModel NewComment { get; set; } = new();

    }
}
