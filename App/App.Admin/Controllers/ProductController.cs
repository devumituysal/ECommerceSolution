using App.Admin.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IAdminService _adminService;

        public ProductController(IProductService productService,IAdminService adminService)
        {
            _productService = productService;
            _adminService = adminService;
        }

        public async Task<IActionResult> List(int? categoryId, string? search)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.GetAdminProductsAsync(jwt,categoryId,search);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Products could not be loaded.";
                return View(new List<ProductListViewModel>());
            }

            var model = result.Value.Select(p => new ProductListViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryName = p.CategoryName,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                Enabled = p.Enabled
            })
            .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.DeleteAsync(jwt, id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Status switch
                {
                    Ardalis.Result.ResultStatus.NotFound
                        => "Product not found.",

                    Ardalis.Result.ResultStatus.Unauthorized
                        => "You are not authorized to perform this action.",

                    Ardalis.Result.ResultStatus.Error when !string.IsNullOrEmpty(result.Errors?.FirstOrDefault())
                        => result.Errors.First(),

                    _ => "Product could not be deleted."
                };

                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.DisableProductAsync(jwt, id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] =
                    result.Status == Ardalis.Result.ResultStatus.NotFound
                        ? "Product not found."
                        : "Product could not be disabled.";

                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Product disabled successfully.";
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Enable(int id)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _adminService.EnableProductAsync(jwt, id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] =
                    result.Status == Ardalis.Result.ResultStatus.NotFound
                        ? "Product not found."
                        : "Product could not be enabled.";

                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Product enabled successfully.";
            return RedirectToAction(nameof(List));
        }


    }
}
