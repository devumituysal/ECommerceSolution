using App.Eticaret.Models.ViewModels;
using App.Models.DTO.Comment;
using App.Services.Abstract;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Eticaret.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [Authorize(Roles ="buyer,seller")]
        [HttpPost("comment/create")]
        public async Task<IActionResult> Create(int productId,SaveProductCommentViewModel newProductCommentModel)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return Unauthorized();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please select a rating before submitting your comment.";
                return Redirect($"/product/{productId}/details");
            }

            var dto = new CreateProductCommentRequestDto
            {
                StarCount = newProductCommentModel.StarCount,
                Text = newProductCommentModel.Text
            };

            var result = await _commentService.CreateAsync(jwt, productId, dto);

            if (result.Status == ResultStatus.NotFound)
            {
                TempData["Error"] = result.Errors.FirstOrDefault()
                        ?? "Comment could not be created.";
                return RedirectToAction("Index", "Home");
            }

            if (!result.IsSuccess)
            {
                TempData["Error"] = "Comment could not be created.";
                return Redirect($"/product/{productId}/details");
            }

            TempData["Success"] = "Your comment has been sent for approval.";

            return Redirect($"/product/{productId}/details?fromAddComment=true");
        }

    }
}
