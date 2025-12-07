using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
