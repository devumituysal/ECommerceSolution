using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Favorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    public class FavoriteController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public FavoriteController(IDataRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [HttpPost("{productId}")]
        public async Task<IActionResult> Add(int productId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var exists = await _repo.GetAll<FavoriteEntity>()
                .AnyAsync(f => f.UserId == userId && f.ProductId == productId);

            if (exists)
                return BadRequest("Product already in favorites");

            var favorite = new FavoriteEntity
            {
                UserId = userId,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.Add(favorite);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var favorite = await _repo.GetAll<FavoriteEntity>()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ProductId == productId);

            if (favorite is null)
                return Ok();

            await _repo.Delete(favorite);

            return Ok();
        }

        [Authorize]
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> IsFavorite(int productId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isFavorite = await _repo.GetAll<FavoriteEntity>()
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);

            return Ok(isFavorite);
        }

        [HttpGet("most-favorited")]
        public async Task<IActionResult> GetMostFavorited([FromQuery] int take = 10)
        {
            take = Math.Clamp(take, 1, 50);

            var products = await _repo.GetAll<FavoriteEntity>()
                .GroupBy(f => f.Product)
                .Select(g => new MostFavoritedProductDto
                {
                    ProductId = g.Key.Id,
                    Name = g.Key.Name,
                    Price = g.Key.Price,
                    FavoriteCount = g.Count(),
                    
                    ImageUrl = g.Key.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.Url)
                    .FirstOrDefault()
                })
                .OrderByDescending(x => x.FavoriteCount)
                .Take(take)
                .ToListAsync();

            return Ok(products);
        }


    }
}
