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

            var result = await _productService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    newProductModel.Categories = categoriesResult.Value;

                ModelState.AddModelError("", "The product could not be created.");
                return View(newProductModel);
            }

            var productId = result.Value;

            if (newProductModel.Images != null && newProductModel.Images.Any())
            {
                var uploadResult =
                    await _productService.UploadImagesAsync(productId, newProductModel.Images.ToList());

                if (!uploadResult.IsSuccess)
                {
                    var categoriesResult = await _categoryService.GetAllAsync();
                    if (categoriesResult.IsSuccess)
                        newProductModel.Categories = categoriesResult.Value;

                    ModelState.AddModelError("", "Product images could not be uploaded.");
                    return View(newProductModel);
                }
            }

            TempData["SuccessMessage"] = "The product has been created successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            var result = await _productService.GetByIdAsync(productId);

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

            var result = await _productService.UpdateAsync(productId, dto);

            
            if (!result.IsSuccess)
            {
                var categoriesResult = await _categoryService.GetAllAsync();
                if (categoriesResult.IsSuccess)
                    editProductModel.Categories = categoriesResult.Value;

                TempData["ErrorMessage"] = "The product could not be updated.";
                return View(editProductModel);
            }

            
            TempData["SuccessMessage"] = "The product has been updated successfully.";
            return RedirectToAction("Edit", new { productId });
        }


        [HttpPost("Delete/{productId:int}")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete(int productId)
        {
            var result = await _productService.DeleteAsync(productId);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "The product could not be deleted.";
                return RedirectToAction("MyProducts", "Profile");
            };

            TempData["SuccessMessage"] = "The product has been successfully deleted.";
            return RedirectToAction("MyProducts", "Profile");

        }

        
        [HttpPost("{productId:int}/add-images")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> AddImages([FromRoute] int productId, [FromForm] List<IFormFile> images)
        {
            if (images == null || !images.Any())
            {
                TempData["ErrorMessage"] = "Please select at least one image.";
                return RedirectToAction("MyProducts", "Profile");
            }

            var result =
                await _productService.UploadImagesAsync(productId, images);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Images could not be loaded.";
                return RedirectToAction("MyProducts", "Profile");
            }

            TempData["SuccessMessage"] = "Images added successfully.";
            return RedirectToAction("MyProducts", "Profile");
        }

        [HttpPost("{productId:int}/delete-image")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> DeleteImage([FromRoute] int productId, [FromForm] string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                TempData["ErrorMessage"] = "No image has been selected to be deleted.";
                return RedirectToAction("MyProducts", "Profile");
            }
           
            var result =
                await _productService.DeleteImageAsync(productId, fileName);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "The image could not be deleted.";
                return RedirectToAction("MyProducts", "Profile");
            }

            TempData["SuccessMessage"] = "The image has been successfully deleted.";
            return RedirectToAction("MyProducts", "Profile");
        }

    }
}
