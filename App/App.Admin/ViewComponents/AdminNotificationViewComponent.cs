using App.Models.DTO.Admin;
using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.ViewComponents
{
    public class AdminNotificationViewComponent : ViewComponent
    {
        private readonly IAdminService _adminService;

        public AdminNotificationViewComponent(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return View(new AdminNotificationDto());

            var dto = await _adminService.GetNotificationsAsync(jwt)
                      ?? new AdminNotificationDto();

            return View(dto);
        }
    }
}
