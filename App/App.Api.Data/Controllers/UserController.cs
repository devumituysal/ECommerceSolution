using App.Api.Data.Models.Dtos.User;
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
    [Authorize(Roles = "Admin")]
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
                .Where(u => u.RoleId != 1)
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role.Name,
                    Enabled = u.Enabled,
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
            user.RoleId = 2; // Seller

            await _repo.Update(user);

            return Ok(new { message = "Kullanıcı başarıyla onaylandı." });
        }
    }
}
