using App.Api.Data.Models.Dtos.Auth;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public AuthController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _repo.GetAll<UserEntity>()
                .Include(u=>u.Role)
                .FirstOrDefaultAsync(u=>u.Email == loginRequestDto.Email && u.Password == loginRequestDto.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var response = new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.Name,
            };

            return Ok(response);
        }


    }
}
