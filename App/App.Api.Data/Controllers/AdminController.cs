using App.Models.DTO.Admin;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/admin")]
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
    }
}
