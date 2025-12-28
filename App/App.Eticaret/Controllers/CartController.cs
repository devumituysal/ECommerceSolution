using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Cart;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    [Authorize(Roles = "buyer, seller")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost("/add-to-cart")]
        public async Task<IActionResult> AddProduct(int productId, byte quantity)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            if (quantity < 1)
                quantity = 1;

            var result = await _cartService.AddProductAsync(jwt, productId, quantity);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Product could not be added to cart.";
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();

            if (prevUrl is null)
                return RedirectToAction(nameof(Edit));

            return Redirect(prevUrl);
        }



        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _cartService.GetMyCartAsync(jwt);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Could not load cart items.";
                return View(new List<CartItemViewModel>());
            }

            var cartItems = result.Value.Select(x => new CartItemViewModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                ProductImage = x.ProductImage,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList();

            return View(cartItems);
        }


        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {

            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _cartService.UpdateItemAsync(
                jwt,
                cartItemId,
                new UpdateCartItemDto { Quantity = quantity }
            );

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Cart item could not be updated.";
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost("/cart/remove")]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _cartService.RemoveItemAsync(jwt, cartItemId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Product could not be removed from cart.";
            }

            return RedirectToAction(nameof(Edit));
        }


        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var jwt = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _cartService.GetMyCartAsync(jwt);
            if (!result.IsSuccess || !result.Value.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(Edit));
            }

            var cartItems = result.Value.Select(x => new CartItemViewModel
            {
                Id = x.Id,
                ProductName = x.ProductName,
                ProductImage = x.ProductImage,
                Quantity = x.Quantity,
                Price = x.Price
            }).ToList();

            var checkoutModel = new CheckoutViewModel
            {
                CartItems = cartItems ?? new List<CartItemViewModel>(),
                Address = "" // boş bırakabiliriz veya kullanıcı adresini çekebiliriz
            };

            ViewBag.CartTotal = cartItems.Sum(x => x.TotalPrice);

            return View(checkoutModel); // artık CheckoutViewModel gönderiyoruz
        }



    }
}
