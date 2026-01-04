using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.ViewComponents
{
    public class ActiveSellersViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public ActiveSellersViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _adminService.GetActiveSellersAsync();

            var model = result.IsSuccess
                ? result.Value.Select(x => new ActiveSellerViewModel
                {
                    SellerFullName = x.SellerFullName,
                    TotalProduct = x.TotalProduct,
                    TotalSales = x.TotalSales,
                    TotalEarning = x.TotalEarning
                }).ToList()
                : new();

            return View(model);
        }
    }
}
