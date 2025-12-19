using App.Api.Data.Models.Dtos.Product;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public ProductController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductRequestDto createProductRequestDto)
        {
            var product = new ProductEntity
            {
                Name = createProductRequestDto.Name,
                Price = createProductRequestDto.Price,
                Details = createProductRequestDto.Details,
                StockAmount = createProductRequestDto.StockAmount,
                CategoryId = createProductRequestDto.CategoryId,
                SellerId = createProductRequestDto.SellerId  // claimden alınmalı
            };

            await _repo.Add(product);

            return Ok(new ProductCreateResponseDto{ProductId = product.Id});
        }

        [HttpPut("{productId:int}")] // productId sadece int girilirse Route düşsün
        public async Task<IActionResult> Update([FromBody] UpdateProductRequestDto updateProductRequestDto, int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                return NotFound();
            }

            product.Name = updateProductRequestDto.Name;
            product.Price = updateProductRequestDto.Price;
            product.Details = updateProductRequestDto.Details;
            product.StockAmount = updateProductRequestDto.StockAmount;
            product.CategoryId = updateProductRequestDto.CategoryId;

            await _repo.Update(product);

            return NoContent(); // başarılı ama dönecek veri yok
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> Delete(int productId)
        {
            var product = await _repo
                .GetAll<ProductEntity>()
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            await _repo.Delete(product);

            return NoContent();
        }

        [HttpPost("{productId:int}/comment")]
        public async Task<IActionResult> CreateComment(int productId,[FromBody] CreateProductCommentRequestDto createProductCommentRequest)
        {
            var productExists = await _repo
                .GetAll<ProductEntity>()
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return NotFound();
            }

            var comment = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = 1, // şimdilik // jwt tamamlandığında düzeltilecek...
                StarCount = createProductCommentRequest.StarCount,
                Text = createProductCommentRequest.Text
            };

            await _repo.Add(comment);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _repo.GetAll<ProductEntity>()
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.Images
                    .OrderBy(i => i.Id)
                    .Select(i => i.Url)
                    .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Details = p.Details,
                    StockAmount = p.StockAmount,
                    CategoryId = p.CategoryId
                })
                .FirstOrDefaultAsync();

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost("{productId:int}/images")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> UploadImages([FromForm] List<IFormFile> images,int productId)
        {
            if (images == null || images.Count == 0)
            {
                return BadRequest("No images uploaded");
            }
                
            var productExists = await _repo.GetAll<ProductEntity>()
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return NotFound();
            }
                
            foreach (var image in images)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var path = Path.Combine("wwwroot/uploads", fileName);

                await using var stream = new FileStream(path, FileMode.Create);
                await image.CopyToAsync(stream);

                await _repo.Add(new ProductImageEntity
                {
                    ProductId = productId,
                    Url = $"/uploads/{fileName}"
                });
            }

            return NoContent();
        }
    }
}
