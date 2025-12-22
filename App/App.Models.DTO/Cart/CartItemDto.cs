namespace App.Models.DTO.Cart
{
    public class CartItemDto
    {
        public int Id { get; set; }               
        public int ProductId { get; set; }       
        public string ProductName { get; set; }   
        public string? ProductImage { get; set; } 
        public byte Quantity { get; set; }      
        public decimal Price { get; set; }
    }
}
