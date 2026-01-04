using App.Models.DTO.Comment;
using App.Models.DTO.Product;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
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
        public CommentService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor) 
        {
            
        }

        // GET /api/comment
        public async Task<Result<List<CommentListItemDto>>> GetAllAsync()
        {
            var response = await SendAsync(HttpMethod.Get,"api/comment");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Comments could not be loaded.");

            var comments = await response.Content.ReadFromJsonAsync<List<CommentListItemDto>>();

            return Result.Success(comments ?? new List<CommentListItemDto>());
        }

        // POST /api/comment/{id}/create

        public async Task<Result> CreateAsync(int productId,CreateProductCommentRequestDto dto)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/comment/{productId}/create",
                dto
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Product not found.");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(errorMessage))
                    return Result.Error(errorMessage);

                return Result.Error("Invalid comment data.");
            }

            if (!response.IsSuccessStatusCode)
                return Result.Error("Comment could not be created.");

            return Result.Success();
        }

        // POST /api/comment/{id}/approve
        public async Task<Result> ApproveAsync(int commentId)
        {
            var response = await SendAsync(HttpMethod.Post,$"api/comment/{commentId}/approve");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Comment could not be approved.");

            return Result.Success();
        }


    }
}
