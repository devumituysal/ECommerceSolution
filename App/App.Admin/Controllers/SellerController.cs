using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("admin/sellers")]
    public class SellerController : Controller
    {
        private readonly IAdminService _adminService;

        public SellerController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> ActiveSellers()
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.GetActiveSellersAsync(jwt);

            if (!result.IsSuccess)
                return View(new List<ActiveSellerViewModel>());

            var model = result.Value.Select(x => new ActiveSellerViewModel
            {
                SellerFullName = x.SellerFullName,
                TotalProduct = x.TotalProduct,
                TotalSales = x.TotalSales,
                TotalEarning = x.TotalEarning
            }).ToList();

            return View(model);
        }
    }
}
