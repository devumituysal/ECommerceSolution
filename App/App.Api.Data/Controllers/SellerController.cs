using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Seller;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SellerController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public SellerController(IDataRepository repo)
        {
            _repo = repo;
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<SellerDetailDto>> GetSellerDetail(int id)
        {
            var seller = await _repo.GetAll<UserEntity>()
                .Include(u => u.Role)
                .Where(u => u.Id == id && u.Role.Name == "Seller" && u.Enabled)
                .Select(u => new SellerDetailDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Products = new List<SellerProductDto>()
                })
                .FirstOrDefaultAsync();

            if (seller == null)
                return NotFound();

            seller.Products = await _repo.GetAll<ProductEntity>()
                .Include(p => p.Images)
                .Where(p => p.SellerId == id)
                .Select(p => new SellerProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.Images
                        .OrderBy(i => i.Id)
                        .Select(i => i.Url)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(seller);
        }
    }
}
