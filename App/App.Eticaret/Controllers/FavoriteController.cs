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
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            await _favoriteService.AddAsync(jwt, productId);

            var prevUrl = Request.Headers.Referer.FirstOrDefault();
            return prevUrl is null
                ? RedirectToAction("Index", "Shop")
                : Redirect(prevUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Remove(int productId)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            await _favoriteService.RemoveAsync(jwt, productId);

            var prevUrl = Request.Headers.Referer.FirstOrDefault();
            return prevUrl is null
                ? RedirectToAction("Index", "Shop")
                : Redirect(prevUrl);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> IsFavorite(int productId)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return Json(false);

            var isFav = await _favoriteService.IsFavoriteAsync(jwt, productId);
            return Json(isFav);
        }
    }
}
