using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : BaseController
    {
        public ProductController(HttpClient httpClient) : base(httpClient) { }

        public async Task<IActionResult> Delete(int id)
        {
            
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            SetJwtHeader();

            var response = await _httpClient.DeleteAsync($"https://localhost:5001/api/products/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index"); 
            }

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Product could not be deleted.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Product deleted successfully.";
            return RedirectToAction("Index");
        }

    }
}
