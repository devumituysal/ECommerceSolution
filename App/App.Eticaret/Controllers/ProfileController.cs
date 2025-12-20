using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "seller, buyer")]
    public class ProfileController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProfileController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            var profile = await _httpClient.GetFromJsonAsync<ProfileDetailsViewModel>
                ("https://localhost:7200/api/profile");

            return View(profile);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editMyProfileModel);
            }

            var response = await _httpClient.PutAsJsonAsync(
                "https://localhost:7200/api/profile", editMyProfileModel);

            if (!response.IsSuccessStatusCode)
            {
                // API'den gelen hata mesajını oku (BadRequest durumunda)
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    if (error != null && error.ContainsKey("message"))
                    {
                        ModelState.AddModelError("", error["message"]);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Profil güncellenemedi.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Profil güncellenemedi.");
                }

                return View(editMyProfileModel);
            }

            TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
            return RedirectToAction(nameof(Details));


          
        }

        [HttpGet("/my-orders")]
        public async Task<IActionResult> MyOrders()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _httpClient.GetAsync("https://localhost:7200/api/order/my-orders");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Siparişler alınamadı.";
                return View(new List<OrderViewModel>());
            }

            var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();

            orders ??= new List<OrderViewModel>();  // null kontrolü

            return View(orders ?? new List<OrderViewModel>());

        }

        [HttpGet("/my-products")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> MyProducts()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _httpClient.GetAsync("https://localhost:7200/api/product");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Ürünler alınamadı.";
                return View(new List<MyProductsViewModel>());
            }

            var products = await response.Content.ReadFromJsonAsync<List<MyProductsViewModel>>();

            products ??= new List<MyProductsViewModel>(); // null kontrolü

            return View(products);
        }

    }
}
