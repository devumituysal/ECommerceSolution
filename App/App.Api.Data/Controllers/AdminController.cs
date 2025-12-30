using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Admin;
using App.Models.DTO.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public AdminController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("notifications")]
        public async Task<ActionResult<AdminNotificationDto>> GetNotifications()
        {
            var newUsersCount = await _repo.GetAll<UserEntity>()
                .CountAsync(u => !u.Enabled  && !u.IsBanned);

            var sellerRequestCount = await _repo.GetAll<UserEntity>()
                .CountAsync(u => u.HasSellerRequest && !u.IsBanned);

            var dto = new AdminNotificationDto
            {
                NewUsers = newUsersCount,
                SellerRequests = sellerRequestCount
            };

            return Ok(dto);
        }

        [HttpGet("products")]
        public async Task<ActionResult<List<ProductListItemDto>>> GetProducts([FromQuery] int? categoryId,[FromQuery] string? q)
        {
            var productsQuery = _repo.GetAll<ProductEntity>()
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
                productsQuery = productsQuery
                    .Where(p => p.CategoryId == categoryId.Value);

            if (!string.IsNullOrWhiteSpace(q))
                productsQuery = productsQuery
                    .Where(p => p.Name.Contains(q));

            var products = await productsQuery
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                })
                .ToListAsync();

            return Ok(products);
        }

        
        [HttpDelete("products/{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            await _repo.Delete(product);
            return NoContent();
        }


    }
}
