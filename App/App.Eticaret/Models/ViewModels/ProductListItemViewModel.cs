namespace App.Eticaret.Models.ViewModels
{
    public class ProductListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? ImageUrl { get; set; }
    }
}
