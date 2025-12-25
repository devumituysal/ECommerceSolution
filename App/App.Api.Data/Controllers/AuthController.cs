using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await _repo.GetAll<UserEntity>()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == loginRequestDto.Email);

            if (user == null)
            {
                return NotFound("Email veya şifre yanlış");
            }

            var hasher = new PasswordHasher<UserEntity>();

            var verifyResult = hasher.VerifyHashedPassword(user,user.Password,loginRequestDto.Password);

            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return NotFound("Email veya şifre yanlış");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),   
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(HttpContext.RequestServices.GetRequiredService<IConfiguration>()["Jwt:Secret"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddMinutes(60);

            var token = new JwtSecurityToken(
                issuer: "App.Api.Data",       
                audience: "App",              
                claims: claims,
                expires: expires,
                signingCredentials: creds
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.Name,
                Token = tokenString
            };

            return Ok(response);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _repo.GetAll<UserEntity>()
                .AnyAsync(u=>u.Email == registerRequestDto.Email);
            
            if(existingUser)
            {
                return BadRequest("Email already exists");
            }

            var hasher = new PasswordHasher<UserEntity>();

            var user = new UserEntity
            {
                FirstName = registerRequestDto.FirstName,
                LastName = registerRequestDto.LastName,
                Email = registerRequestDto.Email,
                Password = hasher.HashPassword(null!, registerRequestDto.Password),
                RoleId =  3,
                Enabled = true,
                HasSellerRequest = false,
            };

            await _repo.Add(user);

            return Ok();
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u=>u.Email==forgotPasswordRequestDto.Email);

            if(user is null)
            {
                return Ok(); // güvenlik sebebi ile ok dönüyoruz!
            }
            
            var resetToken = Guid.NewGuid().ToString("n");

            user.ResetPasswordToken = resetToken;

            await _repo.Update(user);

            // ŞİMDİLİK: mail gönderimi burada olacak
            // (SMTP kodunu birazdan buraya taşıyacağız)

            return Ok();
        }

        [HttpPost("renew-password")]
        [AllowAnonymous]
        public async Task<IActionResult> RenewPassword([FromBody] RenewPasswordRequestDto forgotPasswordRequestDto)
        {
            var user = await _repo.GetAll<UserEntity>()
                .FirstOrDefaultAsync(u=>u.ResetPasswordToken == forgotPasswordRequestDto.Token);

            if(user is null)
            {
                return BadRequest("Invalid or expired token");
            };

            user.Password = forgotPasswordRequestDto.NewPassword;
            user.ResetPasswordToken = null;

            return Ok();
        }

    }
}
