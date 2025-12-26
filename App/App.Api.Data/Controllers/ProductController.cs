using App.Models.DTO.Product;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Seller")]
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
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);


            var product = new ProductEntity
            {
                Name = createProductRequestDto.Name,
                Price = createProductRequestDto.Price,
                Details = createProductRequestDto.Details,
                StockAmount = createProductRequestDto.StockAmount,
                CategoryId = createProductRequestDto.CategoryId,
                SellerId = userId
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
        [Authorize(Roles = "Buyer,Seller")]
        public async Task<IActionResult> CreateComment(int productId,[FromBody] CreateProductCommentRequestDto createProductCommentRequest)
        {
            var productExists = await _repo
                .GetAll<ProductEntity>()
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);

            var comment = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = userId,
                StarCount = createProductCommentRequest.StarCount,
                Text = createProductCommentRequest.Text
            };

            await _repo.Add(comment);

            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetById(int productId)
        {
            var product = await _repo.GetAll<ProductEntity>()
                .Include(p => p.Images)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                return NotFound();

            var dto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Details = product.Details,
                Price = product.Price,
                StockAmount = product.StockAmount,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Images = product.Images
                    .Select(i => i.Url)
                    .ToList()
            };

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyProducts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.Sid)!.Value);

            var products = await _repo.GetAll<ProductEntity>()
                .Include(p => p.Images)
                .Include(p => p.Category)
                .Where(p => p.SellerId == userId) 
                .Select(p => new ProductListItemDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Details = p.Details,
                    Stock = p.StockAmount,
                    CreatedAt = p.CreatedAt,
                    CategoryName = p.Category.Name,
                    ImageUrl = p.Images
                        .OrderBy(i => i.Id)
                        .Select(i => i.Url)
                        .FirstOrDefault(),
                    SellerId = p.SellerId
                })
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return Ok(products);
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
                

                await _repo.Add(new ProductImageEntity
                {
                    ProductId = productId,
                    Url = $"/uploads/{image}"
                });
            }

            return NoContent();
        }

        [HttpDelete("{productId:int}/images")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> DeleteImage(int productId, [FromQuery] string fileName)
        {
            var productImage = await _repo.GetAll<ProductImageEntity>()
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.Url.EndsWith(fileName));

            if (productImage == null)
                return NotFound();

            await _repo.Delete(productImage);

            return NoContent();
        }


    }
}
