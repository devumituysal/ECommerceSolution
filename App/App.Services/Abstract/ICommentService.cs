using App.Models.DTO.Comment;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ICommentService
    {
        Task<Result<List<CommentListItemDto>>> GetAllAsync();
        Task<Result> ApproveAsync(int commentId);
        Task<Result> CreateAsync(int productId, CreateProductCommentRequestDto dto);
    }
}
