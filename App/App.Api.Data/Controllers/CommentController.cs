using App.Api.Data.Models.Dtos.Comment;
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
    public class CommentController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public CommentController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var comments = await _repo.GetAll<CommentEntity>()
                .Select(c => new CommentListItemDto
                {
                    Id = c.Id,
                    Text = c.Text,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    Approved = c.Approved,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(comments);
        }

        [HttpPost("{commentId:int}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int commentId)
        {
            var comment = await _repo.GetAll<CommentEntity>()
                .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return NotFound();
            }

            comment.Approved = true;
            await _repo.Update(comment);

            return Ok(new { message = "Comment approved." });
        }
    }
}

