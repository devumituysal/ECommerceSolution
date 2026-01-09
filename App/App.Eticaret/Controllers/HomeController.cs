using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Comment;
using App.Models.DTO.Contact;
using App.Models.DTO.Product;
using App.Services.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace App.Eticaret.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;
        private readonly IFavoriteService _favoriteService;

        public HomeController(IContactService contactService,IProductService productService,ICategoryService categoryService,ICommentService commentService,IFavoriteService favoriteService)
        {
            _contactService = contactService;
            _productService = productService;
            _categoryService = categoryService;
            _commentService = commentService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var categoryResult = await _categoryService.GetCategoriesWithFirstProductImageAsync();
            var productsResult = await _productService.GetLatestAsync(8);
            var featuredResult = await _favoriteService.GetMostFavoritedAsync(8);

            var vm = new HomeIndexViewModel
            {
                Categories = categoryResult,
                LatestProducts = productsResult,
                FeaturedProducts = featuredResult
            };

            return View(vm);
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
                    result.Errors.FirstOrDefault() ?? "The message could not be sent."
                );
                return View(contactViewModel);
            }

            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction(nameof(Contact));

        }

        [Route("/products/list")]
        public async Task<IActionResult> Listing(int? categoryId, string? q, bool fromMyFavorites = false, int page = 1)
        {
           
            if(!fromMyFavorites)
            {
                TempData.Remove("ErrorFavorite");
            }

            const int pageSize = 12;

            var result = await _productService.GetPublicProductsAsync(categoryId,q);

            if (!result.IsSuccess || result.Value == null)
                return View(new List<ProductListItemViewModel>());

            var totalCount = result.Value.Count;

            var pagedProducts = result.Value
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var products = new List<ProductListItemViewModel>();

            foreach (var p in pagedProducts)
            {
                bool isFavorite = await _favoriteService.IsFavoriteAsync(p.Id);
               
                products.Add(new ProductListItemViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.CategoryName,
                    IsFavorite = isFavorite,
                });
            }

            string? selectedCategoryName = null;

            if (categoryId.HasValue && result.Value.Any())
            {
                selectedCategoryName = result.Value.First().CategoryName;
            }

            ViewBag.SelectedCategoryName = selectedCategoryName;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            ViewBag.CategoryId = categoryId;
            ViewBag.Query = q;

            return View(products);
        }


        [HttpGet("/product/{productId:int}/details")]
        public async Task<IActionResult> ProductDetail([FromRoute] int productId, bool fromAddtoCart = false, bool fromAddComment = false)
        {
            if (!fromAddtoCart)
            {
                TempData.Remove("AddtoCartError");
                TempData.Remove("AddtoCartSuccess");
            }
            if (!fromAddComment)
            {
                TempData.Remove("Error");
                TempData.Remove("Success");
            }

            var result = await _productService.GetPublicByIdAsync(productId);

            if (!result.IsSuccess)
                return NotFound();

            var dto = result.Value;

            var productDetail = new ProductDetailViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                SellerId = dto.SellerId,
                SellerName = dto.SellerName,
                Details = dto.Details,
                Price = dto.Price,
                CategoryName = dto.CategoryName,
                ImageUrls = dto.Images.Count > 0 ? new[] { dto.Images[0] } : Array.Empty<string>(),
                StockAmount = dto.StockAmount,
                Comments = new List<ProductCommentViewModel>(),
                IsFavorite = false
            };

            
            productDetail.IsFavorite = await _favoriteService.IsFavoriteAsync(productId);
            

            var commentResult = await _commentService.GetAllAsync();

                if (commentResult.IsSuccess)
                {
                    productDetail.Comments = commentResult.Value
                        .Where(c => c.IsConfirmed && c.ProductId == productId)
                        .Select(c => new ProductCommentViewModel
                        {
                            Id = c.Id,
                            Content = c.Text,
                            UserName = c.FirstName + " " + c.LastName,
                            CreatedDate = c.CreatedAt,
                            StarCount = c.StarCount
                        })
                        .ToList();
                }
            

            return View(productDetail);

        }
    }   
}
