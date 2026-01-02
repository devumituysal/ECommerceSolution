using App.Data.Entities;
using App.Models.DTO.Admin;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ContactController : Controller
    {
        private readonly IAdminService _adminService;

        public ContactController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> List()
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            var result = await _adminService.GetContactMessagesAsync(jwt);

            if (!result.IsSuccess)
                return View(new List<AdminContactMessageDto>());

            return View(result.Value);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var jwt = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            var result = await _adminService.GetContactByIdAsync(jwt, id);

            if (!result.IsSuccess)
                return NotFound();

            return View(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var jwt = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            var result = await _adminService.DeleteContactAsync(jwt, id);

            TempData["Success"] = result.IsSuccess
                ? "Message deleted successfully."
                : "Message could not be deleted.";

            return RedirectToAction(nameof(List));
        }
    }
}
