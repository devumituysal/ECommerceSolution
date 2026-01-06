using App.Models.DTO.User;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class UserController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public UserController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<UserListItemDto>>> List()
        {
            var users = await _repo.GetAll<UserEntity>()
                .Include(u => u.Role)
                .Where(u => u.Role.Name != "admin") // opsiyonel
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Enabled = u.Enabled,
                    IsBanned = u.IsBanned,
                    HasSellerRequest = u.HasSellerRequest
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve([FromRoute] int id)
        {
            var user = await _repo.GetAll<UserEntity>().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.HasSellerRequest)
            {
                return BadRequest("Kullanıcı satıcı talebinde bulunmamış.");
            }

            user.HasSellerRequest = false;
            user.RoleId = 2;

            await _repo.Update(user);

            return Ok(new { message = "Kullanıcı başarıyla onaylandı." });
        }

        [HttpPost("{id:int}/revoke-seller")]
        public async Task<IActionResult> RevokeSeller([FromRoute] int id)
        {
            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.RoleId != 2) 
                return BadRequest("Kullanıcı satıcı değil.");

            user.RoleId = 3;
            user.HasSellerRequest = false;
            
            var products = await _repo.GetAll<ProductEntity>()
                .Where(p => p.SellerId == user.Id)
                .ToListAsync();

            foreach (var product in products)
            {
                product.Enabled = false;
            }

            await _repo.Update(user);

            return Ok(new { message = "Kullanıcının satıcılığı iptal edildi." });
        }

        [HttpPost("{id:int}/enable")]
        public async Task<IActionResult> Enable([FromRoute] int id)
        {
            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            user.Enabled = true;
            user.IsBanned = false;
            await _repo.Update(user);

            return Ok();
        }

        [HttpPost("{id:int}/disable")]
        public async Task<IActionResult> Disable([FromRoute] int id)
        {
            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound();

            user.Enabled = false;
            user.IsBanned = true;
            await _repo.Update(user);

            return Ok();
        }
    }
}
