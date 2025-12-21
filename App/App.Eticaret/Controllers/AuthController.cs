using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
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
    public class AuthController : BaseController 
    {

        public AuthController(IHttpClientFactory httpClientFactory) 
            : base(httpClientFactory.CreateClient("DataApi")){}


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

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/auth/register",
                new
                {
                    firstName = newUser.FirstName,
                    lastName = newUser.LastName,
                    email = newUser.Email,
                    password = newUser.Password
                });

            if(response.StatusCode == HttpStatusCode.BadRequest)
            {
                ModelState.AddModelError("", "Bu e-posta adresi zaten kayıtlı.");
                return View(newUser);
            }

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu.");
                return View(newUser);
            }

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
        public async Task<IActionResult> Login([FromForm] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
           

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/auth/login",
                new
                {
                    email = loginModel.Email,
                    password = loginModel.Password,
                });

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                ModelState.AddModelError("", "Email veya şifre Hatalı");
                return View(loginModel);
            }

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Bir hata oluştu.");
                return View(loginModel);
            }

            var user = await response.Content.ReadFromJsonAsync<LoginResponseViewModel>();

            if (user is null)
            {
                ModelState.AddModelError("", "Giriş işlemi başarısız.");
                return View(loginModel);
            }

            Response.Cookies.Append("access_token", user.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            await LoginUser(user);

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
            {
                return View(model);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/auth/forgot-password",
                new
                {
                    email = model.Email
                });

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Bir hata oluştu.");
                return View(model);
            }

            ViewBag.SuccessMessage = "Şifre sıfırlama maili gönderildi. Lütfen e-posta adresinizi kontrol edin.";

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
            if(!ModelState.IsValid)
            {
                return View(renewPasswordViewModel);
            }

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/auth/renew-password",
                new
                {
                    token = renewPasswordViewModel.Token,
                    newPassword = renewPasswordViewModel.NewPassword
                });

            if(!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Şifre yenileme işlemi başarısız.");
                return View(renewPasswordViewModel);
            }

            return RedirectToAction("Login", "Auth");
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
    }
}
