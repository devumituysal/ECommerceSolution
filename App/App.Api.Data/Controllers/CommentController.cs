using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Comment;
using App.Models.DTO.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public CommentController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _repo.GetAll<ProductCommentEntity>()
                .Include(c => c.User)
                .Select(c => new CommentListItemDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    IsConfirmed = c.IsConfirmed,
                    CreatedAt = c.CreatedAt,
                    ProductId = c.ProductId,
                    StarCount = c.StarCount,
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost("{productId:int}/create")]
        [Authorize(Roles = "buyer,seller")]
        public async Task<IActionResult> CreateComment(int productId,[FromBody] CreateProductCommentRequestDto request)
        {
            if (request == null)
                return BadRequest();

            if (request.StarCount < 1 || request.StarCount > 5)
                return BadRequest("StarCount must be between 1 and 5.");

            var productExists = await _repo
                .GetAll<ProductEntity>()
                .AnyAsync(p => p.Id == productId);

            if (!productExists)
                return NotFound("Product not found.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            
            var alreadyCommented = await _repo.GetAll<ProductCommentEntity>()
                .AnyAsync(c => c.ProductId == productId && c.UserId == userId);

            if (alreadyCommented)
                return BadRequest("You have already commented on this product.");
            

            var comment = new ProductCommentEntity
            {
                ProductId = productId,
                UserId = userId,
                Text = request.Text.Trim(),
                StarCount = request.StarCount,
                IsConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.Add(comment);

            return NoContent();
        }

        [HttpPost("{commentId:int}/approve")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Approve(int commentId)
        {
            var comment = await _repo.GetAll<ProductCommentEntity>()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            comment.IsConfirmed = true;
            await _repo.Update(comment);

            return Ok(new { message = "Comment approved." });
        }
    }
}

