using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.ViewComponents
{
    public class TotalEarningViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public TotalEarningViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _adminService.GetTotalEarningsAsync();

            var model = result.IsSuccess
                ? new TotalEarningViewModel
                {
                    AnnualEarning = result.Value.AnnualEarning,
                    MonthlyEarning = result.Value.MonthlyEarning
                }
                : new();

            return View(model);
        }
    }
}
