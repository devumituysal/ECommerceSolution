using App.Admin.Models.ViewModels;
using App.Models.DTO.Comment;
using App.Services.Abstract;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/comment")]
    [Authorize(Roles = "admin")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _commentService.GetAllAsync();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Comments could not be loaded.";
                return View(new List<CommentListItemViewModel>());
            }

            var viewModels = (result.Value ?? new List<CommentListItemDto>())
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentListItemViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Approved = c.IsConfirmed,
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            if (!viewModels.Any())
                TempData["InfoMessage"] = "No comments found.";

            return View(viewModels);
        }



        [Route("{commentId:int}/approve")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int commentId)
        {
            var result = await _commentService.ApproveAsync(commentId);

            if (result.Status == ResultStatus.NotFound)
            {
                TempData["ErrorMessage"] = "Comment not found.";
                return RedirectToAction(nameof(List));
            }

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Comment could not be approved.";
                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Comment approved successfully.";
            return RedirectToAction(nameof(List));
        }

    }
}
