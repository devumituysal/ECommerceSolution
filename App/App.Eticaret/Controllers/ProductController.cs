using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static App.Eticaret.Controllers.ProfileController;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController : BaseController
    {
        public ProductController(HttpClient httpClient) : base(httpClient) { }

        [HttpGet("create")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create()
        {
            SetJwtHeader();

            var categories = await _httpClient
                .GetFromJsonAsync<List<CategoryListItemViewModel>>(
                "https://localhost:7200/api/categories");

            ViewBag.CategoryList = categories;

            return View(new ProductSaveViewModel());

        }

        [HttpPost("create")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create([FromForm] ProductSaveViewModel newProductModel)
        {
            if (!ModelState.IsValid)
            {   
                return View(newProductModel);
            }

            SetJwtHeader();

            var sellerIdClaim = User.FindFirst(ClaimTypes.Sid);

            if(sellerIdClaim == null)
            {
                return RedirectToAction(nameof(AuthController.Login), "Auth");
            }

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:7200/api/product",
                new
                {
                    name = newProductModel.Name,
                    price = newProductModel.Price,
                    details = newProductModel.Details,
                    stockAmount = newProductModel.StockAmount,
                    categoryId = newProductModel.CategoryId,
                    sellerId = int.Parse(sellerIdClaim.Value)
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Ürün oluşturulamadı.");
                return View(newProductModel);
            }

            var result = await response.Content.ReadFromJsonAsync<ProductCreateResponseViewModel>();

            if (result == null)
            {
                ModelState.AddModelError("", "Ürün oluşturuldu ama cevap okunamadı.");
                return View(newProductModel);
            }

            int productId = result.ProductId;

            if (newProductModel.Images != null && newProductModel.Images.Any())
            {

                foreach (var image in newProductModel.Images)
                {
                    SetJwtHeader();

                    var content = new MultipartFormDataContent();

                    content.Add(
                        new StreamContent(image.OpenReadStream()),
                        "file",
                        image.FileName
                    );

                    var fileResponse = await _httpClient.PostAsync(
                        "https://localhost:7132/api/file/upload",
                        content
                    );

                    if (!fileResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("", "Ürün görselleri yüklenemedi.");
                        return View(newProductModel);
                    }

                    var uploadResult = await fileResponse.Content.ReadFromJsonAsync<FileUploadResponse>();

                    var imageSaveResponse = await _httpClient.PostAsJsonAsync(
                        $"https://localhost:7200/api/product/{productId}/images",
                        new 
                        { 
                            fileName = uploadResult!.FileName 
                        }
                    );

                    if (!imageSaveResponse.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("", "Ürün görseli kaydedilemedi.");
                        return View(newProductModel);
                    }


                }
            }

            ViewBag.SuccessMessage = "Ürün başarıyla oluşturuldu.";

            return View(new ProductSaveViewModel());
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            SetJwtHeader();

            var product = await _httpClient.GetFromJsonAsync<ProductSaveViewModel>(
                $"https://localhost:7200/api/products/{productId}");

            if (product == null)
            {
                return NotFound();
            }

            var categories = await _httpClient
                .GetFromJsonAsync<List<CategoryListItemViewModel>>(
                "https://localhost:7200/api/categories");

            ViewBag.CategoryList = categories;

            return View(product);

        }

        [HttpPost("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId, [FromForm] ProductSaveViewModel editProductModel)
        {
            SetJwtHeader();

            var categories = await _httpClient
                .GetFromJsonAsync<List<CategoryListItemViewModel>>(
                "https://localhost:7200/api/categories");

            ViewBag.CategoryList = categories;

            if (!ModelState.IsValid)
            {
                return View(editProductModel);
            }

            var response = await _httpClient.PutAsJsonAsync(
                $"https://localhost:7200/api/product/{productId}",
                new
                {
                    name = editProductModel.Name,
                    price = editProductModel.Price,
                    details = editProductModel.Details,
                    stockAmount = editProductModel.StockAmount,
                    categoryId = editProductModel.CategoryId
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Ürün güncellenemedi.");
                return View(editProductModel);
            }

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi.";

            return View(editProductModel);
        }

        [HttpGet("{productId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {
            SetJwtHeader();

            var response = await _httpClient.DeleteAsync(
                $"https://localhost:7200/api/product/{productId}");

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            ViewBag.SuccessMessage = "Ürün başarıyla silindi.";

            return View();

        }

        [HttpPost("{productId:int}/comment")]
        [Authorize(Roles = "buyer, seller")]
        public async Task<IActionResult> Comment([FromRoute] int productId, [FromForm] SaveProductCommentViewModel newProductCommentModel)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            SetJwtHeader();

            var response = await _httpClient.PostAsJsonAsync($"https://localhost:7200/api/product/{productId}/comment",
                new
                {
                    starCount = newProductCommentModel.StarCount,
                    text = newProductCommentModel.Text
                });

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
