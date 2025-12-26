namespace App.Models.DTO.Product
{
    public class ProductListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string Details { get; set; } = "";
        public byte Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CategoryName { get; set; } = "";
        public string? ImageUrl { get; set; } = "";
        public int SellerId { get; set; }
    }
}
