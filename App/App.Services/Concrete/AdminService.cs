using App.Models.DTO.Admin;
using App.Models.DTO.Product;
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
    public class AdminService : BaseService , IAdminService
    {
        public AdminService(IHttpClientFactory factory) : base(factory)
        {

        }

        public async Task<AdminNotificationDto?> GetNotificationsAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/admin/notifications",
                jwt
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<AdminNotificationDto>();
        }
        public async Task<Result<List<ProductListItemDto>>> GetAdminProductsAsync(string jwt, int? categoryId, string? search)
        {
            var url = "api/Admin/products";
            var queryParams = new List<string>();

            if (categoryId.HasValue)
                queryParams.Add($"categoryId={categoryId.Value}");

            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"q={WebUtility.UrlEncode(search)}");

            if (queryParams.Any())
                url += "?" + string.Join("&", queryParams);

            var response = await SendAsync(
                HttpMethod.Get,
                url,
                jwt
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Products could not be loaded.");

            var products =
                await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();

            return Result.Success(products ?? new());
        }

        public async Task<Result> DeleteAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/Admin/products/{productId}",
                jwt
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

    }
}
