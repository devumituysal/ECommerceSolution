using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> List()
        {
            List<UserListItemViewModel> users = await _dbContext.Users
               .Where(u => u.RoleId != 1)
               .Select(u => new UserListItemViewModel
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

            return View(users);
        }


        [Route("/users/{id:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> Approve([FromRoute] int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(i=>i.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if (!user.HasSellerRequest)
            {
                return BadRequest();
            }

            user.HasSellerRequest = false;
            user.RoleId = 2; // seller

            await _dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(List));
        }

    }
}
