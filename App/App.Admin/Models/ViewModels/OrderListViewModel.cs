namespace App.Admin.Models.ViewModels
{
    public class OrderListViewModel
    {
        public string OrderNumber { get; set; } = null!;
        public string UserFullName { get; set; } = null!;
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
