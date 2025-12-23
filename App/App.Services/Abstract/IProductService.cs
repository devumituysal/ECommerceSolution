using App.Models.DTO.Product;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App.Services.Abstract
{
    public interface IProductService
    {
        Task<Result<int>> CreateAsync(string jwt, CreateProductRequestDto dto);
        Task<Result> UpdateAsync(string jwt, int productId, UpdateProductRequestDto dto);
        Task<Result> DeleteAsync(string jwt, int productId);
        Task<Result> CreateCommentAsync(string jwt, int productId, CreateProductCommentRequestDto dto);
        Task<Result<List<ProductListItemDto>>> GetMyProductsAsync(string jwt);
        Task<Result> UploadImagesAsync(string jwt, int productId, List<IFormFile> files);
        Task<Result> DeleteImageAsync(string jwt, int productId, string fileName);
        Task<Result<ProductDetailDto>> GetByIdAsync(string jwt, int productId);

        Task<Result<List<ProductListItemDto>>> GetPublicProductsAsync();
        Task<Result<ProductDetailDto>> GetPublicByIdAsync(int productId);

    }
}
