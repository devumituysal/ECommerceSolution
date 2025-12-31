namespace App.Admin.Models.ViewModels
{
    public class ProductListViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string CategoryName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool Enabled { get; set; }
    }
}
