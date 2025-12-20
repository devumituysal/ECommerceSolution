namespace App.Api.Data.Models.Dtos.Product
{
    public class ProductDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Details { get; set; } = null!;
        public decimal Price { get; set; }
        public int StockAmount { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<string> Images { get; set; } = new();
        public int CategoryId { get; set; }
    }
}
