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
                    Enabled = p.Enabled,
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("orders")]
        public async Task<ActionResult<List<OrderListDto>>> GetOrders()
        {
            var orders = await _repo.GetAll<OrderEntity>()
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderListDto
                {
                    OrderNumber = o.OrderCode,

                    UserFullName = o.User.FirstName + " " + o.User.LastName,

                    TotalPrice = o.OrderItems
                        .Sum(oi => oi.Quantity * oi.UnitPrice),

                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return Ok(orders);
        }



        [HttpDelete("products/{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            var hasOrders = await _repo.GetAll<OrderItemEntity>()
                .AnyAsync(oi => oi.ProductId == productId);

            if (hasOrders)
                return BadRequest("This product has orders and cannot be deleted.");

            await _repo.Delete(product);
            return NoContent();
        }

        [HttpPatch("products/{productId:int}/disable")]
        public async Task<IActionResult> DisableProduct(int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            product.Enabled = false;
            await _repo.Update(product);

            return NoContent();
        }

        [HttpPatch("products/{productId:int}/enable")]
        public async Task<IActionResult> EnableProduct(int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            product.Enabled = true;
            await _repo.Update(product);

            return NoContent();
        }




        [HttpGet("active-sellers")]
        public IActionResult GetActiveSellers()
        {
            var result = _repo.GetAll<OrderItemEntity>()
                .Include(oi => oi.Product)
                .ThenInclude(p => p.Seller)
               .Select(oi => new
               {
                   SellerId = oi.Product.Seller.Id,
                   SellerFullName = oi.Product.Seller.FirstName + " " + oi.Product.Seller.LastName,
                   ProductId = oi.ProductId,
                   Quantity = oi.Quantity,
                   Earning = oi.Quantity * oi.UnitPrice
               })
                .GroupBy(x => new { x.SellerId, x.SellerFullName })
                .Select(g => new ActiveSellerDto
                {
                    SellerFullName = g.Key.SellerFullName,

                    // satıcının sattığı farklı ürün sayısı
                    TotalProduct = g
                        .Select(x => x.ProductId)
                        .Distinct()
                        .Count(),

                    // toplam satış adedi (quantity)
                    TotalSales = g.Sum(x => x.Quantity),

                    // satıcının toplam kazancı
                    TotalEarning = g.Sum(x => x.Earning)
                })
                .OrderByDescending(x => x.TotalEarning)
                .Take(5)
                .ToList();

            return Ok(result);
        }

        [HttpGet("total-earnings")]
        public IActionResult GetTotalEarnings()
        {
            var now = DateTime.UtcNow;

            var items = _repo.GetAll<OrderItemEntity>()
                .Include(oi => oi.Order);

            var annual = items
                .Where(x => x.Order.CreatedAt.Year == now.Year)
                .Sum(x => x.Quantity * x.UnitPrice);

            var monthly = items
                .Where(x =>
                    x.Order.CreatedAt.Year == now.Year &&
                    x.Order.CreatedAt.Month == now.Month)
                .Sum(x => x.Quantity * x.UnitPrice);

            var result = new TotalEarningDto
            {
                AnnualEarning = annual,
                MonthlyEarning = monthly
            };

            return Ok(result);
        }



    }
}
