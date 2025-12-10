using System.Diagnostics;
using App.Data.Contexts;
using App.Data.Entities;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Eticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _dbContext;

        public HomeController(AppDbContext dbContext)
        {
            _dbContext= dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/about-us")]
        public IActionResult AboutUs()
        {
            return View();
        }

        [HttpGet("/contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("/contact")]
        public  async Task<IActionResult> Contact([FromForm] ContactViewModel contactViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(contactViewModel);
            }
            
            var contactMessage = new ContactMessageEntity
            {
                Name = contactViewModel.Name,
                Email = contactViewModel.Email,
                Message = contactViewModel.Message,
            };

            _dbContext.ContactMessages.Add(contactMessage);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your message has been successfully sent!";
            return RedirectToAction("Contact");

        }

        [Route("/products/list")]    
        public IActionResult Listing()
        {
            return View();
        }


        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var product = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p=>p.Id == productId);

            if (product is null)
            {
                return NotFound();      
            }

            var productView = new ProductDetailViewModel
            {
                Id = product.Id,
                Name= product.Name,
                CategoryName = product.Category.Name,
                Price = product.Price,
                ImageUrls = product.Images
                        .Where(i => !string.IsNullOrWhiteSpace(i.Url))
                        .OrderBy(i => i.Id)
                        .Select(i => i.Url)
                        .ToArray()
            };
            
            return View(productView);
        }
    }
}
