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
            if (quantity < 1)
                quantity = 1;

            var result = await _cartService.AddProductAsync(productId, quantity);

            if (!result.IsSuccess)
            {
                TempData["AddtoCartError"] = "An error occurred while adding the product to the cart.";
            }
            else
            {
                TempData["AddtoCartSuccess"] = "Product successfully added to cart.";
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();


            if (string.IsNullOrEmpty(prevUrl))
            {
                return RedirectToAction("Edit", "Cart");
            }

            return Redirect(prevUrl + "?fromAddtoCart=true");
        }



        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var result = await _cartService.GetMyCartAsync();

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
                Price = x.Price,
                ProductStockAmount = x.ProductStockAmount 
            }).ToList();

            return View(cartItems);
        }


        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {
            var result = await _cartService.UpdateItemAsync(cartItemId,new UpdateCartItemDto { Quantity = quantity });

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Cart item could not be updated.";
            }

            return RedirectToAction(nameof(Edit));
        }

        [HttpPost("/cart/remove")]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var result = await _cartService.RemoveItemAsync(cartItemId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Product could not be removed from cart.";
            }

            return RedirectToAction(nameof(Edit));
        }


        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var result = await _cartService.GetMyCartAsync();
            if (!result.IsSuccess || !result.Value.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction(nameof(Edit));
            }

            bool stockAdjusted = false;

            var cartItems = result.Value.Select(x =>
            {
                // Eğer servis modelinde stok bilgisi varsa burası çalışır
                if (x.ProductStockAmount > 0 && x.Quantity > x.ProductStockAmount)
                {
                    stockAdjusted = true;
                }

                return new CartItemViewModel
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    ProductImage = x.ProductImage,
                    Quantity = x.Quantity,
                    Price = x.Price
                };
            }).ToList();

            if (stockAdjusted)
            {
                TempData["StockAdjusted"] =
                    "Sepetinizdeki bazı ürünlerin adedi stok durumuna göre güncellendi. Lütfen siparişinizi kontrol ediniz.";
            }

            var checkoutModel = new CheckoutViewModel
            {
                CartItems = cartItems,
                Address = ""
            };

            ViewBag.CartTotal = cartItems.Sum(x => x.TotalPrice);

            return View(checkoutModel);
        }




    }
}
