using App.Eticaret.Models.ViewModels;
using App.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Add(int productId)
        {
            await _favoriteService.AddAsync(productId);

            var prevUrl = Request.Headers.Referer.FirstOrDefault();
            return prevUrl is null
                ? RedirectToAction("Index", "Shop")
                : Redirect(prevUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Remove(int productId)
        {
            await _favoriteService.RemoveAsync(productId);

            var prevUrl = Request.Headers.Referer.FirstOrDefault();
            return prevUrl is null
                ? RedirectToAction("Index", "Shop")
                : Redirect(prevUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IsFavorite(int productId)
        {
            var isFav = await _favoriteService.IsFavoriteAsync(productId);
            return Json(isFav);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> MyFavorites()
        {
            var result = await _favoriteService.GetMyFavoritesAsync();

            if (result.Status == Ardalis.Result.ResultStatus.Unauthorized)
                return RedirectToAction("Login", "Auth");

            if (!result.IsSuccess)
            {
                TempData["ErrorFavorite"] = "favorites could not be retrieved.";
                return RedirectToAction("Listing", "Home", new { fromMyFavorites = true });
            }

            if (result.Value is null || !result.Value.Any())
            {
                TempData["ErrorFavorite"] = "You haven't added any products to your favorites yet.";
                return RedirectToAction("Listing", "Home" , new { fromMyFavorites = true });
            }

            var model = result.Value.Select(p => new ProductListItemViewModel
            {
                Id = p.ProductId,
                Name = p.Name,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                IsFavorite = true,
                CategoryName = string.Empty 
            }).ToList();

            return View(model );
        }

    }
}
    