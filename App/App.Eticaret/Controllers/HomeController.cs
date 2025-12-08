using System.Diagnostics;
using App.Eticaret.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class HomeController : Controller
    {
   
        public IActionResult Index()
        {
            return View();
        }

        [Route("/products/list")]    
        public IActionResult Listing()
        {
            return View();
        }
    }
}
