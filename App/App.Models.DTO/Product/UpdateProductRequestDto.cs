namespace App.Models.DTO.Product
{
    public class UpdateProductRequestDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Details { get; set; }
        public byte StockAmount { get; set; }
        public int CategoryId { get; set; }
    }
}
