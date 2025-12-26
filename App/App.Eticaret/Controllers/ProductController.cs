using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Eticaret.Models.ApiResponses;
using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Product;
using App.Services.Abstract;
using App.Services.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService,ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }



        [HttpGet("create")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create()
        {
            var categoriesResult = await _categoryService.GetAllAsync();

            var model = new ProductSaveViewModel();

            if (categoriesResult.IsSuccess)
                model.Categories = categoriesResult.Value;

            return View(model);
        }

        [HttpPost("create")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create([FromForm] ProductSaveViewModel newProductModel)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            // ❗ ModelState invalid → dropdown tekrar doldurulmalı
            if (!ModelState.IsValid)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    newProductModel.Categories = categoriesResult.Value;

                return View(newProductModel);
            }

            var dto = new CreateProductRequestDto
            {
                Name = newProductModel.Name,
                Price = newProductModel.Price,
                Details = newProductModel.Details,
                StockAmount = newProductModel.StockAmount,
                CategoryId = newProductModel.CategoryId
            };

            var result = await _productService.CreateAsync(jwt, dto);

            if (!result.IsSuccess)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    newProductModel.Categories = categoriesResult.Value;

                ModelState.AddModelError("", "Ürün oluşturulamadı.");
                return View(newProductModel);
            }

            var productId = result.Value;

            if (newProductModel.Images != null && newProductModel.Images.Any())
            {
                var uploadResult =
                    await _productService.UploadImagesAsync(jwt, productId, newProductModel.Images.ToList());

                if (!uploadResult.IsSuccess)
                {
                    var categoriesResult = await _categoryService.GetAllAsync();
                    if (categoriesResult.IsSuccess)
                        newProductModel.Categories = categoriesResult.Value;

                    ModelState.AddModelError("", "Ürün görselleri yüklenemedi.");
                    return View(newProductModel);
                }
            }

            TempData["SuccessMessage"] = "Ürün başarıyla oluşturuldu.";
            return RedirectToAction("MyProducts", "Profile");
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _productService.GetByIdAsync(jwt, productId);

            if (!result.IsSuccess)
                return NotFound();

            var product = result.Value;

            var categoriesResult = await _categoryService.GetAllAsync();

            var viewModel = new ProductSaveViewModel
            {
                Name = product.Name,
                Price = product.Price,
                Details = product.Details,
                StockAmount = product.StockAmount,
                CategoryId = product.CategoryId,
                ExistingImages = product.Images,
                Categories = categoriesResult.IsSuccess
                    ? categoriesResult.Value
                    : new()
            };

            return View(viewModel);
        }

        [HttpPost("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit(
      [FromRoute] int productId,
      [FromForm] ProductSaveViewModel editProductModel)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            // ❌ Validation hatası
            if (!ModelState.IsValid)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    editProductModel.Categories = categoriesResult.Value;

                return View(editProductModel);
            }

            var dto = new UpdateProductRequestDto
            {
                Name = editProductModel.Name,
                Price = editProductModel.Price,
                Details = editProductModel.Details,
                StockAmount = editProductModel.StockAmount,
                CategoryId = editProductModel.CategoryId
            };

            var result = await _productService.UpdateAsync(jwt, productId, dto);

            // ❌ Güncelleme başarısız
            if (!result.IsSuccess)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    editProductModel.Categories = categoriesResult.Value;

                TempData["ErrorMessage"] = "Ürün güncellenemedi.";
                return View(editProductModel);
            }

            // ✅ Başarılı
            TempData["SuccessMessage"] = "Ürün başarıyla güncellendi.";
            return RedirectToAction("Edit", new { productId });
        }


        [HttpPost("Delete/{productId:int}")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete(int productId)
        {
            var jwt = HttpContext.Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _productService.DeleteAsync(jwt, productId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Ürün silinemedi.";
                return RedirectToAction("MyProducts", "Profile");
            };

            TempData["SuccessMessage"] = "Ürün başarıyla silindi.";
            return RedirectToAction("MyProducts", "Profile");

        }

        [HttpPost("{productId:int}/comment")]
        [Authorize(Roles = "buyer, seller")]
        public async Task<IActionResult> Comment([FromRoute] int productId, [FromForm] SaveProductCommentViewModel newProductCommentModel)
        {

            if (!ModelState.IsValid)
                return BadRequest();

            var jwt = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            var dto = new CreateProductCommentRequestDto
            {
                StarCount = newProductCommentModel.StarCount,
                Text = newProductCommentModel.Text
            };

            var result =
                await _productService.CreateCommentAsync(jwt, productId, dto);

            if (!result.IsSuccess)
                return BadRequest();

            return Ok();
        }

        [HttpPost("{productId:int}/add-images")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> AddImages([FromRoute] int productId, [FromForm] List<IFormFile> images)
        {
            if (images == null || !images.Any())
            {
                TempData["ErrorMessage"] = "Lütfen en az bir görsel seçin.";
                return RedirectToAction("MyProducts", "Profile");
            }

            var jwt = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result =
                await _productService.UploadImagesAsync(jwt, productId, images);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Görseller yüklenemedi.";
                return RedirectToAction("MyProducts", "Profile");
            }

            TempData["SuccessMessage"] = "Görseller başarıyla eklendi.";
            return RedirectToAction("MyProducts", "Profile");
        }

        [HttpPost("{productId:int}/delete-image")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromForm] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["ErrorMessage"] = "Silinecek görsel seçilmedi.";
                return RedirectToAction("MyProducts", "Profile");
            }

            var jwt = HttpContext.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result =
                await _productService.DeleteImageAsync(jwt, productId, fileName);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Görsel silinemedi.";
                return RedirectToAction("MyProducts", "Profile");
            }

            TempData["SuccessMessage"] = "Görsel başarıyla silindi.";
            return RedirectToAction("MyProducts", "Profile");
        }

    }
}
