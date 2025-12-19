using System.Diagnostics;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(IDataRepository repo,HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet("/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("/contact")]
        public  async Task<IActionResult> Contact([FromForm] ContactViewModel contactViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(contactViewModel);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/contact",
                new
                {
                    name = contactViewModel.Name,
                    email = contactViewModel.Email,
                    message = contactViewModel.Message
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Mesaj gönderilemedi.");
                return View(contactViewModel);
            }

            TempData["SuccessMessage"] = "Mesajýnýz baþarýyla gönderildi!";
            return RedirectToAction(nameof(Contact));

        }

        [Route("/products/list")]    
        public async Task<IActionResult> Listing()
        {
            var products = await _httpClient.GetFromJsonAsync<List<ProductListItemViewModel>>(
                "https://localhost:7200/api/products");

            return View(products);
        }


        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var product = await _httpClient
                .GetFromJsonAsync<ProductDetailViewModel>(
                $"https://localhost:7200/api/products/{productId}");

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
          
        }
    }
}
