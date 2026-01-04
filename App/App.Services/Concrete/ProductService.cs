using App.Models.DTO.File;
using App.Models.DTO.Product;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IConfiguration _config;
        public ProductService(IHttpClientFactory factory, IConfiguration config, IHttpContextAccessor httpContextAccessor) : base(factory, httpContextAccessor)
        {
            _config = config;
        }

        // POST /api/product
        public async Task<Result<int>> CreateAsync(CreateProductRequestDto dto)
        {
            var response = await SendAsync(HttpMethod.Post, "api/product",dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result =
                await response.Content.ReadFromJsonAsync<ProductCreateResponseDto>();

            return Result.Success(result!.ProductId);
        }

        // PUT /api/product/{productId}
        public async Task<Result> UpdateAsync(int productId, UpdateProductRequestDto dto)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/product/{productId}", dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // DELETE /api/product/{productId}
        public async Task<Result> DeleteAsync(int productId)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/product/{productId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }


        // GET /api/product
        public async Task<Result<List<ProductListItemDto>>> GetMyProductsAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/product");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result = await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();

            return Result.Success(result!);
        }

        // POST /api/product/{productId}/images
        public async Task<Result> UploadImagesAsync(int productId,List<IFormFile> files)
        {
            var imageUrls = new List<string>();
            var fileApiBaseUrl = _config["ApiSettings:FileApiBaseUrl"];

            foreach (var file in files)
            {
                // 1️⃣ File API'ye upload
                var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(file.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

                content.Add(
                    streamContent,
                    "file",
                    file.FileName);

                var response = await SendAsync(
                    HttpMethod.Post,
                    "api/file/upload",
                    content,
                    useFileClient: true);

                if (!response.IsSuccessStatusCode)
                    return Result.Error("File upload failed");

                var uploadResult =
                    await response.Content
                        .ReadFromJsonAsync<FileUploadResponseDto>();

                imageUrls.Add($"{fileApiBaseUrl}/uploads/{uploadResult!.FileName}");
            }

            // 2️⃣ Data API – toplu ilişkilendir
            var dto = new AddProductImagesRequestDto
            {
                ImageUrls = imageUrls
            };

            var imageResponse = await SendAsync(
                HttpMethod.Post,
                $"api/product/{productId}/images",
                dto
                );


            if (!imageResponse.IsSuccessStatusCode)
                return Result.Error("Image relation failed");

            return Result.Success();
        }



        // DELETE /api/product/{productId}/images?fileName=xxx
        public async Task<Result> DeleteImageAsync(int productId, string fileName)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/product/{productId}/images?fileName={fileName}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();

        }

        // GET /api/product/{productId}
        public async Task<Result<ProductDetailDto>> GetByIdAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"api/product/{productId}"
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result = await response.Content.ReadFromJsonAsync<ProductDetailDto>();

            return Result.Success(result!);
        }

        
        public async Task<Result<List<ProductListItemDto>>> GetPublicProductsAsync(int? categoryId, string? search)
        {
            var url = "api/products";
            var queryParams = new List<string>();

            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"q={WebUtility.UrlEncode(search)}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await DataClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return Result.Error("Products could not be loaded");

            var products =
                await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();

            return Result.Success(products ?? new());
        }

        
        public async Task<Result<ProductDetailDto>> GetPublicByIdAsync(int productId)
        {
            var response = await DataClient.GetAsync($"api/products/{productId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var dto = await response.Content.ReadFromJsonAsync<ProductDetailDto>();
            return Result.Success(dto!);
        }

        public async Task<List<ProductListItemDto>> GetLatestAsync(int take)
        {
            var response = await DataClient.GetAsync($"/api/products/latest?take={take}");

            if (!response.IsSuccessStatusCode)
                return new List<ProductListItemDto>();

            var items = await response.Content
                .ReadFromJsonAsync<List<ProductListItemDto>>();

            return items ?? new List<ProductListItemDto>();
        }





    }
}
