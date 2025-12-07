using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
