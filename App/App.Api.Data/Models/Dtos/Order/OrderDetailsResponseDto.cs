namespace App.Api.Data.Models.Dtos.Order
{
    public class OrderDetailsResponseDto
    {
        public string OrderCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = null!;
        public List<OrderItemDto> Items { get; set; } = new();

    }
    public class OrderItemDto
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
