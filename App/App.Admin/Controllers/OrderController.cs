using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
   
    [Authorize(Roles = "admin")]
    public class OrderController : Controller
    {
        private readonly IAdminService _adminService;

        public OrderController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<IActionResult> List()
        {
            var result = await _adminService.GetAdminOrdersAsync();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Orders could not be loaded.";
                return View(new List<OrderListViewModel>());
            }


            var model = result.Value.Select(p => new OrderListViewModel
            {
               OrderNumber = p.OrderNumber,
               TotalPrice = p.TotalPrice,
               CreatedAt = p.CreatedAt,
               UserFullName = p.UserFullName,
            })
            .ToList();

            return View(model);
        }
    }
}
