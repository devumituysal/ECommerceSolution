namespace App.Eticaret.Models.ViewModels
{
    public class OrderDetailsViewModel
    {
        public string OrderCode { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Address { get; set; } = string.Empty;
        public List<OrderItemViewModel> Items { get; set; } = [];
    }
}
