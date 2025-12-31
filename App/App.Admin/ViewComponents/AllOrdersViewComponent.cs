using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.ViewComponents
{
    public class AllOrdersViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public AllOrdersViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return View(new List<OrderListViewModel>());

            var result = await _adminService.GetAdminOrdersAsync(jwt);

            if (!result.IsSuccess)
                return View(new List<OrderListViewModel>());

            var model = result.Value
                .OrderByDescending(o => o.CreatedAt)
                .Take(5)
                .Select(o => new OrderListViewModel
                {
                    OrderNumber = o.OrderNumber,
                    UserFullName = o.UserFullName,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt
                })
                .ToList();

            return View(model);
        }

    }
}
