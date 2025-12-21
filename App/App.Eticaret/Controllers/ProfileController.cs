using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ApiResponses;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "seller, buyer")]
    public class ProfileController : BaseController
    {
        public ProfileController(HttpClient httpClient) : base(httpClient) { }

        [HttpGet("/profile")]
        public async Task<IActionResult> Details()
        {
            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:7200/api/profile");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Auth");
            }

            var profile = await response.Content.ReadFromJsonAsync<ProfileDetailsViewModel>();

            if (profile != null && !string.IsNullOrEmpty(profile.ProfileImagePath))
            {
                profile.ProfileImageUrl = $"/uploads/{profile.ProfileImage}";
            }

            return View(profile);
        }

        [HttpPost("/profile")]
        public async Task<IActionResult> Edit([FromForm] ProfileDetailsViewModel editMyProfileModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editMyProfileModel);
            }

            SetJwtHeader();

            string? uploadedFileName = null;

            if (editMyProfileModel.ProfileImage != null)
            {
                var content = new MultipartFormDataContent();

                content.Add(
                    new StreamContent(editMyProfileModel.ProfileImage.OpenReadStream()),
                    "file",
                    editMyProfileModel.ProfileImage.FileName
                    );

                var fileResponse = await _httpClient.PostAsync("https://localhost:7132/api/file/upload", content);

                if (!fileResponse.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Profil fotoğrafı yüklenemedi.");
                    return View(editMyProfileModel);
                }

                var fileResult = await fileResponse.Content.ReadFromJsonAsync<FileUploadResponse>();

                uploadedFileName = fileResult!.FileName;
            }


            var response = await _httpClient.PutAsJsonAsync(
                "https://localhost:7200/api/profile", 
                new
                {
                    firstName = editMyProfileModel.FirstName,
                    lastName = editMyProfileModel.LastName,
                    email = editMyProfileModel.Email,
                    profileImage = uploadedFileName
                });

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

            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:7200/api/order/my-orders");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Siparişler alınamadı.";
                return View(new List<OrderViewModel>());
            }

            var orders = await response.Content.ReadFromJsonAsync<List<OrderViewModel>>();

            orders ??= new List<OrderViewModel>();  

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

            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:7200/api/product");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Ürünler alınamadı.";
                return View(new List<MyProductsViewModel>());
            }

            var products = await response.Content.ReadFromJsonAsync<List<MyProductsViewModel>>();

            products ??= new List<MyProductsViewModel>(); 

            return View(products);
        }
    }
}
