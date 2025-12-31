using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Auth;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;


namespace App.Eticaret.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
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
                return View(newUser);

            var dto = new RegisterRequestDto
            {
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Password = newUser.Password
            };

            var result = await _authService.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "Kayıt işlemi başarısız.");
                return View(newUser);
            }

            return RedirectToAction(nameof(Login));
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var dto = new LoginRequestDto
            {
                Email = loginModel.Email,
                Password = loginModel.Password
            };

            var result = await _authService.LoginAsync(dto);

            
            if (result.Status == Ardalis.Result.ResultStatus.Forbidden)
            {
                ModelState.AddModelError("", "Your account is not active.");
                return View(loginModel);
            }

            
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(loginModel);
            }

            var userDto = result.Value;

            var userVm = new LoginResponseViewModel
            {
                Id = userDto.Id,
                Email = userDto.Email,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Role = userDto.Role,
                Token = userDto.Token
            };

            Response.Cookies.Append("access_token", userVm.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            await LoginUser(userVm);

            
            if (userVm.Role == "admin")
                return Redirect("https://localhost:7223/auth/login");

            return RedirectToAction("Index", "Home");
        }



        [Route("/forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [Route("/forgot-password")]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new ForgotPasswordRequestDto
            {
                Email = model.Email
            };

            var result = await _authService.ForgotPasswordAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "Bir hata oluştu.");
                return View(model);
            }

            ViewBag.SuccessMessage =
                "Şifre sıfırlama maili gönderildi. Lütfen e-posta adresinizi kontrol edin.";

            ModelState.Clear();
            return View();
        }

        //private async Task SendResetPasswordEmailAsync(UserEntity user)
        //{
        //    // Gönderici mail bilgileri güncellenmeli
        //    const string host = "smtp.gmail.com";
        //    const int port = 587;
        //    const string from = "mail";
        //    const string password = "şifre";

        //    var resetPasswordToken = Guid.NewGuid().ToString("n");

        //    user.ResetPasswordToken = resetPasswordToken;
        //    await _repo.Update(user);

        //    using SmtpClient client = new(host, port)
        //    {
        //        Credentials = new NetworkCredential(from, password)
        //    };

        //    MailMessage mail = new()
        //    {
        //        From = new MailAddress(from),
        //        Subject = "Şifre Sıfırlama",
        //        Body = $"Merhaba {user.FirstName}, <br> Şifrenizi sıfırlamak için <a href='https://localhost:5001/renew-password/{user.ResetPasswordToken}'>tıklayınız</a>.",
        //        IsBodyHtml = true,
        //    };

        //    mail.To.Add(user.Email);

        //    await client.SendMailAsync(mail);
        //}

        [Route("/renew-password/{token}")]
        [HttpGet]
        public IActionResult RenewPassword([FromRoute] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction(nameof(ForgotPassword));
            }

            var model = new RenewPasswordViewModel
            {
                Token = token
            };

            return View(model);
        }

        [Route("/renew-password")]
        [HttpPost]
        public async Task<IActionResult> RenewPassword([FromForm] RenewPasswordViewModel renewPasswordViewModel)
        {
            if (!ModelState.IsValid)
                return View(renewPasswordViewModel);

            var dto = new RenewPasswordRequestDto
            {
                Token = renewPasswordViewModel.Token,
                NewPassword = renewPasswordViewModel.NewPassword
            };

            var result = await _authService.RenewPasswordAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", "Şifre yenileme işlemi başarısız.");
                return View(renewPasswordViewModel);
            }

            return RedirectToAction(nameof(Login));
        }

        [Route("/logout")]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("access_token");

            await LogoutUser();

            return RedirectToAction(nameof(Login));
        }

        private async Task LogoutUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private async Task LoginUser(LoginResponseViewModel user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("login-time" , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true // (beni hatırla)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
