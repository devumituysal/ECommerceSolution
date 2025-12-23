using App.Models.DTO.Comment;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class CommentService : BaseService , ICommentService
    {
        public CommentService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) 
        {
            
        }

        // GET /api/comment
        public async Task<Result<List<CommentListItemDto>>> GetAllAsync(string jwt)
        {
            var response = await SendAsync(HttpMethod.Get,"api/comment",jwt);

            if (!response.IsSuccessStatusCode)
                return Result.Error("Comments could not be loaded.");

            var comments =
                await response.Content.ReadFromJsonAsync<List<CommentListItemDto>>();

            return Result.Success(comments ?? new List<CommentListItemDto>());
        }

        // POST /api/comment/{id}/approve
        public async Task<Result> ApproveAsync(string jwt, int commentId)
        {
            var response = await SendAsync(HttpMethod.Post,$"api/comment/{commentId}/approve",jwt);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Comment could not be approved.");

            return Result.Success();
        }


    }
}
