using App.Models.DTO.Cart;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Authorize(Roles = "buyer,seller")]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IDataRepository _repo;
        public CartController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("add/{productId:int}")]
        public async Task<IActionResult> AddToCart(int productId, [FromQuery] byte? quantity)
        {
            if (quantity < 1)
                quantity = 1;

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var productExists = await _repo.GetAll<ProductEntity>()
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
                return NotFound("Product not found");

            var cartItem = await _repo.GetAll<CartItemEntity>()
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);

            var qty = quantity.GetValueOrDefault(1);

            if (cartItem != null)
            {
                cartItem.Quantity += qty;
                await _repo.Update(cartItem);
            }
            else
            {
                cartItem = new CartItemEntity
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = qty,
                    CreatedAt = DateTime.UtcNow
                };
                await _repo.Add(cartItem);
            }

            return Ok(cartItem);
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cartItems = await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product.Images)
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartItemDto
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ProductImage = ci.Product.Images.OrderBy(i => i.Id).Select(i => i.Url).FirstOrDefault(),
                    Quantity = ci.Quantity,
                    Price = ci.Product.Price
                })
                .ToListAsync();

            return Ok(cartItems);
        }

        [HttpPut("{cartItemId:int}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] UpdateCartItemDto updateCartItemDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cartItem = await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);

            if (cartItem == null)
                return NotFound();

            cartItem.Quantity = updateCartItemDto.Quantity;
            await _repo.Update(cartItem);

            return NoContent();
        }

        [HttpDelete("{cartItemId:int}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cartItem = await _repo.GetAll<CartItemEntity>()
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);

            if (cartItem == null)
                return NotFound();

            await _repo.Delete(cartItem);

            return NoContent();
        }


        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var cartItems = await _repo.GetAll<CartItemEntity>()
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            // Burada checkout işlemi yapılabilir (order oluşturma vs)
            // Şimdilik sadece cart itemları döndürüyoruz

            return Ok(cartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                Quantity = ci.Quantity,
                Price = ci.Product.Price
            }));
        }

    }
}
