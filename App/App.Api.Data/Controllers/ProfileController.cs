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
                ProfileImage = user.ProfileImage
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDetail)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
     
            var user = await _repo.GetAll<UserEntity>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var emailExists = await _repo.GetAll<UserEntity>()
                .AnyAsync(u => u.Email == updateProfileDetail.Email && u.Id != userId);

            if (emailExists)
            {
                return BadRequest(new { message = "Bu email zaten kayıtlı." });
            }

            user.FirstName = updateProfileDetail.FirstName;
            user.LastName = updateProfileDetail.LastName;
            user.Email = updateProfileDetail.Email;

            if (!string.IsNullOrWhiteSpace(updateProfileDetail.ProfileImage))
            {
                user.ProfileImage = updateProfileDetail.ProfileImage;
            }

            await _repo.Update(user);

            return NoContent();
        }


    }
}
