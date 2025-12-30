using App.Models.DTO.File;
using App.Models.DTO.Product;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public ProductService(IHttpClientFactory factory) : base(factory)
        {

        }

        // POST /api/product
        public async Task<Result<int>> CreateAsync(string jwt, CreateProductRequestDto dto)
        {
            var response = await SendAsync(HttpMethod.Post, "api/product", jwt, dto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result =
                await response.Content.ReadFromJsonAsync<ProductCreateResponseDto>();

            return Result.Success(result!.ProductId);
        }

        // PUT /api/product/{productId}
        public async Task<Result> UpdateAsync(string jwt, int productId, UpdateProductRequestDto dto)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/product/{productId}", jwt, dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // DELETE /api/product/{productId}
        public async Task<Result> DeleteAsync(string jwt, int productId)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/product/{productId}", jwt);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // POST /api/product/{productId}/comment
        public async Task<Result> CreateCommentAsync(string jwt, int productId, CreateProductCommentRequestDto dto)
        {
            var response = await SendAsync(HttpMethod.Post, $"api/product/{productId}/comment", jwt, dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // GET /api/product
        public async Task<Result<List<ProductListItemDto>>> GetMyProductsAsync(string jwt)
        {
            var response = await SendAsync(HttpMethod.Get, "api/product", jwt);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result = await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();

            return Result.Success(result!);
        }

        // POST /api/product/{productId}/images
        public async Task<Result> UploadImagesAsync(
            string jwt,
            int productId,
            List<IFormFile> files)
        {
            var imageUrls = new List<string>();

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

                var uploadRequest = new HttpRequestMessage(
                    HttpMethod.Post,
                    "api/file/upload");

                uploadRequest.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);

                uploadRequest.Content = content;

                var uploadResponse = await FileClient.SendAsync(uploadRequest);

                if (!uploadResponse.IsSuccessStatusCode)
                    return Result.Error("File upload failed");

                var uploadResult =
                    await uploadResponse.Content
                        .ReadFromJsonAsync<FileUploadResponseDto>();


                var fileApiBaseUrl = "https://localhost:7132";
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
                jwt,
                dto);


            if (!imageResponse.IsSuccessStatusCode)
                return Result.Error("Image relation failed");

            return Result.Success();
        }



        // DELETE /api/product/{productId}/images?fileName=xxx
        public async Task<Result> DeleteImageAsync(string jwt, int productId, string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/product/{productId}/images?fileName={fileName}");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await DataClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();

        }

        // GET /api/product/{productId}
        public async Task<Result<ProductDetailDto>> GetByIdAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"api/product/{productId}",
                jwt);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result =
                await response.Content.ReadFromJsonAsync<ProductDetailDto>();

            return Result.Success(result!);
        }

        // home listing sayfası için...login olmayan kullanıcılar... ( jwt siz)
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

        // home detail sayfası için
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

        public async Task<Result<List<ProductListItemDto>>> GetLatestAsync(int count)
        {
            var response = await DataClient.GetAsync($"api/products/latest?count={count}");
            if (!response.IsSuccessStatusCode)
                return Result.Error("Products could not be loaded");

            var products = await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();
            return Result.Success(products ?? new());
        }

      

    }
}
