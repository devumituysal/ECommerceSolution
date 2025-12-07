using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
