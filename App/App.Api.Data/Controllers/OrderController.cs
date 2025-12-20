using App.Api.Data.Models.Dtos.Order;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public OrderController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto createOrderRequestDto)
        {
            var cartItems = await _repo.GetAll<CartItemEntity>()
                .Include(ci=>ci.Product)
                .Where(ci=>ci.UserId == createOrderRequestDto.UserId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                return BadRequest("Cart is empty");
            }

            var order = new OrderEntity
            {
                UserId = createOrderRequestDto.UserId,
                Address = createOrderRequestDto.Address,
                OrderCode = await CreateOrderCode() // direk guid yapmaktansa %100 unique olması için aşağıdaki metodla yapıldı
            };

            await _repo.Add(order);

            foreach(var item in cartItems)
            {
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };

                await _repo.Add(orderItem);
            }

            return Ok(new { orderCode = order.OrderCode });
 
        }
        private async Task<string> CreateOrderCode()
        {
            var orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();

            while (await _repo.GetAll<OrderEntity>().AnyAsync(o=>o.OrderCode == orderCode))
            {
                orderCode = Guid.NewGuid().ToString("n")[..16].ToUpperInvariant();
            }

            return orderCode;
        }

        [HttpGet("{orderCode}")]
        public async Task<IActionResult> GetDetails(string orderCode, [FromQuery] int userId)
        {
            var order = await _repo.GetAll<OrderEntity>()
                .Where(o => o.OrderCode == orderCode && o.UserId == userId)
                .Select(o => new OrderDetailsResponseDto
                {
                    OrderCode = o.OrderCode,
                    CreatedAt = o.CreatedAt,
                    Address = o.Address,
                    Items = o.OrderItems.Select(oi => new OrderItemDto
                    {
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if(order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.Sid)?.Value;

            if (userIdClaim is null)
            {
                return NotFound();
            }

            var userId = int.Parse(userIdClaim);

            var orders = await _repo.GetAll<OrderEntity>()
                .Where(o => o.UserId == userId)
                .Select(o => new OrderDto
                {
                    OrderCode = o.OrderCode,
                    Address = o.Address,
                    CreatedAt = o.CreatedAt,
                    TotalPrice = o.OrderItems.Sum(oi => oi.UnitPrice * oi.Quantity),
                    TotalProducts = o.OrderItems.Count,
                    TotalQuantity = o.OrderItems.Sum(oi => oi.Quantity),
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }


    }
}
