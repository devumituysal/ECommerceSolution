namespace App.Api.Data.Models.Dtos.Product
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryName { get; set; } = null!;
    }
}
