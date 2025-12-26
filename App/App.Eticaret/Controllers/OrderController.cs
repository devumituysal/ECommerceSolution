using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Order;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("/order")]
        public async Task<IActionResult> Create([FromForm] CheckoutViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
            {
                return RedirectToAction("Login", "Auth");
            }

            var request = new CreateOrderRequestDto
            {
                Address = model.Address
            };

            var result = await _orderService.CreateAsync(jwt, request);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "Sipariş oluşturulamadı.");
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { orderCode = result.Value });


        }

        [HttpGet("/order/{orderCode}/details")]
        public async Task<IActionResult> Details([FromRoute] string orderCode)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
            {
                return RedirectToAction("Login", "Auth");
            }

            var result = await _orderService.GetOrderDetailsAsync(jwt, orderCode);

            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return View(result.Value);
        }
    }
}
