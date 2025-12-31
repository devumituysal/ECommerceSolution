namespace App.Admin.Models.ViewModels
{
    public class ActiveSellerViewModel
    {
        public string SellerFullName { get; set; } = null!;
        public int TotalProduct { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalEarning { get; set; }
    }
}
