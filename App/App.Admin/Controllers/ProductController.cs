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

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Delete(int id)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _productService.DeleteAsync(jwt, id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] =
                    result.Status == Ardalis.Result.ResultStatus.NotFound
                        ? "Product not found."
                        : "Product could not be deleted.";

                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}
