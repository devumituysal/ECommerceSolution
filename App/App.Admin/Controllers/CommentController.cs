using App.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Admin.Controllers
{
    [Route("/comment")]
    [Authorize(Roles = "admin")]
    public class CommentController : BaseController
    {
        public CommentController(HttpClient httpClient) : base(httpClient) { }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:5001/api/comment");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Comments could not be loaded.";
                return View(new List<CommentListItemViewModel>());
            }

            var comments = await response.Content.ReadFromJsonAsync<List<CommentListItemViewModel>>();

            comments ??= new List<CommentListItemViewModel>();

            return View(comments);
        }

        [Route("{commentId:int}/approve")]
        [HttpGet]
        public async Task<IActionResult> Approve([FromRoute] int commentId)
        {
            SetJwtHeader();

            var response = await _httpClient.PostAsync($"https://localhost:5001/api/comment/{commentId}/approve", null);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Comment could not be approved.";
                return RedirectToAction(nameof(List));
            }

            TempData["SuccessMessage"] = "Comment approved successfully.";
            return RedirectToAction(nameof(List));
        }
    }
}
