using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    public class ProductController : Controller
    {
      public IActionResult Delete()
        {  
            return View(); 
        }
    }
}
