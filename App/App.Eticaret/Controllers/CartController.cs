using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class CartController : BaseController
    {
        public CartController(HttpClient httpClient) : base(httpClient) { }
        
        [HttpGet("/add-to-cart/{productId:int}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId)
        {
            SetJwtHeader();

            var response = await _httpClient.PostAsync(
                $"https://localhost:7200/api/cart/add/{productId}", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Product could not be added to cart.";
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();

            return prevUrl is null ? RedirectToAction(nameof(Edit)) : Redirect(prevUrl);
        }
        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:7200/api/cart");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Could not load cart items.";
                return View(new List<CartItemViewModel>());
            }

            var cartItems = await response.Content.ReadFromJsonAsync<List<CartItemViewModel>>();

            return View(cartItems ?? new List<CartItemViewModel>());
        }
        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {
            SetJwtHeader();

            var response = await _httpClient.PutAsJsonAsync(
                $"https://localhost:7200/api/cart/{cartItemId}",
                new { Quantity = quantity });

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Cart item could not be updated.";
                return RedirectToAction(nameof(Edit));
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            SetJwtHeader();

            var response = await _httpClient.PostAsync("https://localhost:7200/api/cart/checkout", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Checkout failed.";
                return RedirectToAction(nameof(Edit));
            }

            TempData["SuccessMessage"] = "Checkout successful!";

            return RedirectToAction("Index", "Home"); 
        }

    }
}
