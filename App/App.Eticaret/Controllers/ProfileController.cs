using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ApiResponses;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Order;
using App.Models.DTO.Product;
using App.Models.DTO.Profile;
using App.Services.Abstract;
using App.Services.Concrete;
using Ardalis.Result;
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
            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            var profileResult = await _profileService.GetMyProfileAsync();
            if (!profileResult.IsSuccess)
                return RedirectToAction("Login", "Auth");

            var dto = profileResult.Value;

            var model = new ProfileDetailsViewModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                HasSellerRequest = dto.HasSellerRequest,
            };

            return View(model);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Details));

            var dto = new UpdateProfileDto
            {
                FirstName = editMyProfileModel.FirstName,
                LastName = editMyProfileModel.LastName,
                Email = editMyProfileModel.Email
            };

            var result = await _profileService.UpdateMyProfileAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] =
                    result.Errors.FirstOrDefault() ?? "Profil güncellenemedi.";

                return RedirectToAction(nameof(Details));
            }

            TempData["ProfileSuccessMessage"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Details));
        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            var result = await _orderService.GetMyOrdersAsync();

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
            var result = await _productService.GetMyProductsAsync();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Ürünler alınamadı.";
                return View(new List<ProductListItemDto>());
            }

            return View(result.Value);
        }

        [Authorize(Roles = "buyer")]
        [HttpPost]
        public async Task<IActionResult> RequestSeller()
        {
            var result = await _profileService.RequestSellerAsync();

            if (!result.IsSuccess)
            {
                TempData["ProfileErrorMessage"] =
                    "Seller request could not be sent or already exists.";
            }
            else
            {
                TempData["ProfileSuccessMessage"] =
                    "Seller request sent successfully. Please wait for admin approval.";
            }

            return RedirectToAction(nameof(Details));
        }
    }
}
