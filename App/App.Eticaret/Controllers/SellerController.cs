using App.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class SellerController : Controller
    {
        private readonly ISellerService _sellerService;

        public SellerController(ISellerService sellerService)
        {
            _sellerService = sellerService;
        }

        [HttpGet]
        public async Task<IActionResult> SellerDetail(int id)
        {
            var result = await _sellerService.GetSellerDetailAsync(id);

            if (result.Status == Ardalis.Result.ResultStatus.NotFound)
                return NotFound();

            if (!result.IsSuccess)
                return View("Error");

            return View(result.Value);
        }

    }
}
