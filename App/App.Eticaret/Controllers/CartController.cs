using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    public class CartController : Controller
    {
        private readonly IDataRepository _repo;

        public CartController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("/add-to-cart/{productId:int}")]
        public async Task<IActionResult> AddProduct([FromRoute] int productId)
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            if (!await _repo.GetAll<ProductEntity>().AnyAsync(p => p.Id == productId))
            {
                return NotFound();
            }

            var cartItem = await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product)
                .Include(ci => ci.User)
                .FirstOrDefaultAsync(ci => ci.UserId == int.Parse(userId)/*burdaki int parse ı kontrol et*/&& ci.ProductId == productId);

            if (cartItem is not null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItem = new CartItemEntity
                {
                    UserId = int.Parse(userId), // burdaki int parse kontrol et
                    ProductId = productId,
                    Quantity = 1,
                    CreatedAt = DateTime.UtcNow
                };

                await _repo.Add(cartItem);   
            }

            var prevUrl = Request.Headers.Referer.FirstOrDefault();

            if (prevUrl is null)
            {
                return RedirectToAction(nameof(Edit));
            }

            return Redirect(prevUrl);
        }
        [HttpGet("/cart")]
        public async Task<IActionResult> Edit()
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            List<CartItemViewModel> cartItem = await GetCartItemsAsync();

            return View(cartItem);
        }
        [HttpPost("/cart/update")]
        public async Task<IActionResult> UpdateCart(int cartItemId, byte quantity)
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var cartItem = await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product.Images)
                .FirstOrDefaultAsync(ci => ci.UserId == int.Parse(userId) && ci.Id == cartItemId);

            if (cartItem is null)
            {
                return NotFound();
            }

            cartItem.Quantity = quantity;
            await _repo.Update(cartItem);

            var model = new CartItemViewModel
            {
                Id = cartItem.Id,
                ProductName = cartItem.Product.Name,
                ProductImage = cartItem.Product.Images.Count != 0 ? cartItem.Product.Images.First().Url : null,
                Quantity = cartItem.Quantity,
                Price = cartItem.Product.Price
            };

            return View(model);
        }

        [HttpGet("/checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = "1"; // login olmuş kullanıcının id si yazılmalı.

            if (userId is null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            List<CartItemViewModel> cartItems = await GetCartItemsAsync();

            return View(cartItems);
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync()
        {
            //var userId = GetUserId() ?? -1;
            var userId = 1;// bura değişecek


            return await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product.Images)
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartItemViewModel
                {
                    Id = ci.Id,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.Images.Count != 0 ? ci.Product.Images.First().Url : null,
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                })
                .ToListAsync();
        }
    }
}
