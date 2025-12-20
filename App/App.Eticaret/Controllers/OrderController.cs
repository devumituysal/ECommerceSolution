using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("/order")]
        public async Task<IActionResult> Create([FromForm] CheckoutViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.Sid);

            if (userIdClaim == null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);
            

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/orders",
                new
                {
                    userId = userId,
                    address = model.Address
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Sipariş oluşturulamadı.");
                return View(model);
            }

            var result = await response.Content.ReadFromJsonAsync<OrderCreateResponseViewModel>();

            return RedirectToAction(nameof(Details), new { orderCode = result!.OrderCode });
        }

        [HttpGet("/order/{orderCode}/details")]
        public async Task<IActionResult> Details([FromRoute] string orderCode)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Sid);

            if (userIdClaim == null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);

            var response = await _httpClient.GetAsync(
                $"https://localhost:7200/api/orders/{orderCode}?userId={userId}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var order =
                await response.Content.ReadFromJsonAsync<OrderDetailsViewModel>();

            return View(order);
        }
    }
}
