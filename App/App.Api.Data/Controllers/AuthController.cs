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

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _repo.GetAll<UserEntity>()
                .AnyAsync(u=>u.Email == registerRequestDto.Email);
            
            if(existingUser)
            {
                return BadRequest("Email already exists");
            }

            var user = new UserEntity
            {
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                Email = registerRequestDto.Email,
                Password = registerRequestDto.Password,
                RoleId =  2,
                Enabled = true,
                HasSellerRequest = false,
            };

            await _repo.Add(user);

            return Ok();
        }
            



    }
}
