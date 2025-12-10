using App.Data.Contexts;
using App.Data.Entities;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace App.Eticaret.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [Route("/register")]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterUserViewModel newUser)
        {
            if (!ModelState.IsValid)
            {
                return View(newUser);
            }

            var user = new UserEntity
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,    
                Email = newUser.Email,
                Password = newUser.Password,
                RoleId = 2,
                Enabled = true,
                HasSellerRequest = false,
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Login", "Auth");
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public IActionResult Login([FromForm] LoginViewModel loginModel)
        {

            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            var user = _dbContext.Users
                .FirstOrDefault(u => u.Email == loginModel.Email && u.Password == loginModel.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Email veya şifre hatalı.");
                return View(loginModel);
            }


            // (Cookie ekle)

            return RedirectToAction("Index", "Home");
        }

    }
}
