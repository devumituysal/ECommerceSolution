using App.Data.Contexts;
using App.Data.Entities;
using App.Eticaret.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace App.Eticaret.Controllers
{
    [Route("/product")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _dbContext;

        public ProductController(AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        [HttpGet("")]
        [Authorize(Roles = "seller")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Create([FromForm] ProductSaveViewModel newProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(newProductModel);
            }

            var product = new ProductEntity
            {
                Name = newProductModel.Name,
                Price = newProductModel.Price,
                Details = newProductModel.Details,
                StockAmount = newProductModel.StockAmount,
                SellerId = 1, // TODO: login olmuş userId al...
                CategoryId = newProductModel.CategoryId,

            };

            await _dbContext.Products.AddAsync(product);

            await _dbContext.SaveChangesAsync();

            await SaveProductImages(product.Id, newProductModel.Images);

            return View();
        }

        [HttpGet("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId)
        {
            var productEntity = await _dbContext.Products.FirstOrDefaultAsync(p=>p.Id == productId);
            if (productEntity is null)
            {
                return NotFound();
            }

            var viewModel = new ProductSaveViewModel
            {
                CategoryId = productEntity.CategoryId,
                Name = productEntity.Name,
                Price = productEntity.Price,
                Details = productEntity.Details,
                StockAmount = productEntity.StockAmount
            };

            return View(viewModel);
        }

        [HttpPost("{productId:int}/edit")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Edit([FromRoute] int productId, [FromForm] ProductSaveViewModel editProductModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editProductModel);
            }

            var productEntity = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (productEntity is null)
            {
                return NotFound();
            }

            productEntity.CategoryId = editProductModel.CategoryId;
            productEntity.Name = editProductModel.Name;
            productEntity.Price = editProductModel.Price;
            productEntity.Details = editProductModel.Details;
            productEntity.StockAmount = editProductModel.StockAmount;

            await _dbContext.SaveChangesAsync();

            ViewBag.SuccessMessage = "Ürün başarıyla güncellendi.";

            return View(editProductModel);
        }

        [HttpGet("{productId:int}/delete")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> Delete([FromRoute] int productId)
        {

            var deleteProduct = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if(deleteProduct is not null)
            {
                _dbContext.Products.Remove(deleteProduct);
                await _dbContext.SaveChangesAsync();
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

            var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (product is null)
            {
                return NotFound();
            }


            var productCommentEntity = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = 1, // TODO: login olmuş userId al...
                Text = newProductCommentModel.Text,
                StarCount = newProductCommentModel.StarCount,
            };

            await _dbContext.ProductComments.AddAsync(productCommentEntity);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }








        //////////////////////////////////////////TOOLS///////////////////////////////////////////////////////////


        public async Task SaveProductImages(int productId , IList<IFormFile> images)
        {
            foreach(var image in images)
            {
                var productImageEntity = new ProductImageEntity
                {
                    ProductId = productId,
                    Url = $"/uploads/{Guid.NewGuid()}{Path.GetExtension(image.FileName)}"
                };
                await _dbContext.AddAsync(productImageEntity);

                await using var fileStream = new FileStream($"wwwroot{productImageEntity.Url}", FileMode.Create);
                await image.CopyToAsync(fileStream);
            }

        }

    }
}
