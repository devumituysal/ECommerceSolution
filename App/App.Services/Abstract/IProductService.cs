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
        Task<Result<int>> CreateAsync(CreateProductRequestDto dto);
        Task<Result> UpdateAsync(int productId, UpdateProductRequestDto dto);
        Task<Result> DeleteAsync(int productId);
        Task<Result<List<ProductListItemDto>>> GetMyProductsAsync();
        Task<Result> UploadImagesAsync(int productId, List<IFormFile> files);
        Task<Result> DeleteImageAsync(int productId, string fileName);
        Task<Result<ProductDetailDto>> GetByIdAsync(int productId);

        Task<Result<List<ProductListItemDto>>> GetPublicProductsAsync(int? categoryId, string? q);
        Task<Result<ProductDetailDto>> GetPublicByIdAsync(int productId);

        Task<List<ProductListItemDto>> GetLatestAsync(int count);

        

    }
}
