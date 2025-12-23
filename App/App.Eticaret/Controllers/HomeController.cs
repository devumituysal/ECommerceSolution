using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Contact;
using App.Services.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace App.Eticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IProductService _productService;

        public HomeController(IContactService contactService,IProductService productService)
        {
            _contactService = contactService;
            _productService = productService;
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
                return View(contactViewModel);

            var dto = new CreateContactMessageDto
            {
                Name = contactViewModel.Name,
                Email = contactViewModel.Email,
                Message = contactViewModel.Message
            };

            var result = await _contactService.SendMessageAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    "",
                    result.Errors.FirstOrDefault() ?? "Mesaj gönderilemedi."
                );
                return View(contactViewModel);
            }

            TempData["SuccessMessage"] = "Mesajýnýz baþarýyla gönderildi!";
            return RedirectToAction(nameof(Contact));

        }

        [Route("/products/list")]    
        public async Task<IActionResult> Listing()
        {
            var result = await _productService.GetPublicProductsAsync();

            if (!result.IsSuccess)
                return View(new List<ProductListItemViewModel>());

            var products = result.Value
                .Select(p => new ProductListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl
                })
                .ToList();

            return View(products);
        }


        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId)
        {
            var result = await _productService.GetPublicByIdAsync(productId);

            if (!result.IsSuccess)
                return NotFound();

            var dto = result.Value;

            var productDetail = new ProductDetailViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Details = dto.Details,
                Price = dto.Price,
                CategoryName = dto.CategoryName,
                ImageUrls = dto.Images.Count > 0? new[] { dto.Images[0] }: Array.Empty<string>()};

            return View(productDetail);

        }
    }   
}
