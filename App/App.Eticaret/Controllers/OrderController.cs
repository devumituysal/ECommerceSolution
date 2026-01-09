using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Order;
using App.Services.Abstract;
using App.Services.Concrete;
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
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService ,ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        [HttpPost("/order")]
        public async Task<IActionResult> Create([FromForm] CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Checkout view'ına geri dönerken CartItems listesini tekrar doldur
               
                var resultCart = await _cartService.GetMyCartAsync();
                model.CartItems = resultCart?.Value?.Select(x => new CartItemViewModel
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList() ?? new List<CartItemViewModel>();

                return View("~/Views/Cart/Checkout.cshtml", model);
            }


            var request = new CreateOrderRequestDto
            {
                Address = model.Address
            };

            var result = await _orderService.CreateAsync(request);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "The order could not be created.");

                // Hata durumunda da CartItems listesini doldur
                var resultCart = await _cartService.GetMyCartAsync();
                model.CartItems = resultCart?.Value?.Select(x => new CartItemViewModel
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList() ?? new List<CartItemViewModel>();

                return View("~/Views/Cart/Checkout.cshtml", model);
            }

            return RedirectToAction(nameof(Details), new { orderCode = result.Value });
        }


        [HttpGet("/order/{orderCode}/details")]
        public async Task<IActionResult> Details([FromRoute] string orderCode)
        {
            var result = await _orderService.GetOrderDetailsAsync(orderCode);

            var dto = result.Value;

            var vm = new OrderDetailsViewModel
            {
                OrderCode = dto.OrderCode,
                Address = dto.Address,
                CreatedAt = dto.CreatedAt,
                Items = dto.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return View(vm);
        }
    }
}
