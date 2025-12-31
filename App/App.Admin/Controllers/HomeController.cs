using App.Admin.Models;
using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class HomeController : Controller
    {
        private readonly IAdminService _adminService;

        public HomeController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IActionResult> Index()
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.GetAdminOrdersAsync(jwt);

            var orders = result.IsSuccess
                ? result.Value.Select(o => new OrderListViewModel
                {
                    OrderNumber = o.OrderNumber,
                    UserFullName = o.UserFullName,
                    TotalPrice = o.TotalPrice,
                    CreatedAt = o.CreatedAt
                }).ToList()
                : new List<OrderListViewModel>();

            return View(orders);
        }
    }
}
