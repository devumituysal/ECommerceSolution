using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Delete(int id)
        {
            
            if (!User.Identity!.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

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
