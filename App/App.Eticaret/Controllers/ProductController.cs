using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
