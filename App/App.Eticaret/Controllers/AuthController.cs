using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
