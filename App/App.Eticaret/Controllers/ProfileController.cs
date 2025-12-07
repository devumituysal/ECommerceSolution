using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
