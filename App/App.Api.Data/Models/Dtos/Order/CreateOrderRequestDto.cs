namespace App.Api.Data.Models.Dtos.Order
{
    public class CreateOrderRequestDto
    {
        public int UserId { get; set; }
        public string Address { get; set; } = null!;
    }
}
