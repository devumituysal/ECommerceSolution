using App.Models.DTO.Profile;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public ProfileController(IDataRepository repo)
        {
            _repo = repo;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _repo.GetAll<UserEntity>()
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if(user == null)
            {
                return NotFound();
            }

            return Ok(new ProfileDetailDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.Name,
                HasSellerRequest = user.HasSellerRequest
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDetail)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim.Value);

            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var emailExists = await _repo.GetAll<UserEntity>()
                .AnyAsync(u => u.Email == updateProfileDetail.Email && u.Id != userId);

            if (emailExists)
                return BadRequest(new { message = "Bu email zaten kayıtlı." });

            user.FirstName = updateProfileDetail.FirstName;
            user.LastName = updateProfileDetail.LastName;
            user.Email = updateProfileDetail.Email;

            await _repo.Update(user);

            return NoContent();
        }

        [Authorize]
        [HttpPost("request-seller")]
        public async Task<IActionResult> RequestSeller()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                return NotFound();

            if (user.HasSellerRequest)
                return BadRequest(new { message = "Seller request already sent." });

            user.HasSellerRequest = true;
            await _repo.Update(user);

            return Ok();
        }



    }
}
