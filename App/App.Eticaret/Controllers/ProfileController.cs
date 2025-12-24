using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ApiResponses;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Profile;
using App.Services.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "seller, buyer")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public ProfileController(IProfileService profileService,IOrderService orderService,IProductService productService)
        {
            _profileService = profileService;
            _orderService = orderService;
            _productService = productService;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _profileService.GetMyProfileAsync(jwt);

            if (!result.IsSuccess)
                return RedirectToAction("Login", "Auth");

            var dto = result.Value;

            var profileDetail = new ProfileDetailsViewModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                ProfileImagePath = dto.ProfileImage,
                ProfileImageUrl = string.IsNullOrEmpty(dto.ProfileImage)
                    ? null
                    : $"/uploads/{dto.ProfileImage}"
            };

            return View(profileDetail);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            if (!ModelState.IsValid)
                return View(editMyProfileModel);

            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var dto = new ProfileDetailDto
            {
                FirstName = editMyProfileModel.FirstName,
                LastName = editMyProfileModel.LastName,
                Email = editMyProfileModel.Email,
                ProfileImage = editMyProfileModel.ProfileImagePath
            };

            var result = await _profileService.UpdateMyProfileAsync(jwt, dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.FirstOrDefault() ?? "Profil güncellenemedi.");
                return View(editMyProfileModel);
            }

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Details));



        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _orderService.GetMyOrdersAsync(jwt);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Siparişler alınamadı.";
                return View(new List<OrderViewModel>());
            }

            return View(result.Value);

        }

        [HttpGet("/my-products")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> MyProducts()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _productService.GetMyProductsAsync(jwt);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Ürünler alınamadı.";
                return View(new List<MyProductsViewModel>());
            }

            return View(result.Value);
        }
    }
}
