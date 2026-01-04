using App.Models.DTO.Admin;
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
    public class AdminService : BaseService , IAdminService
    {
        public AdminService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory, httpContextAccessor)
        {
        }

        public async Task<AdminNotificationDto?> GetNotificationsAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/admin/notifications"
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<AdminNotificationDto>();
        }
        public async Task<Result<List<ProductListItemDto>>> GetAdminProductsAsync(int? categoryId, string? search)
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
                url
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Products could not be loaded.");

            var products =
                await response.Content.ReadFromJsonAsync<List<ProductListItemDto>>();

            return Result.Success(products ?? new());
        }

        public async Task<Result> DeleteAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/Admin/products/{productId}"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Error("This product has orders and cannot be deleted.");

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }


        public async Task<Result> DisableProductAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Patch,
                $"api/Admin/products/{productId}/disable"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Product could not be disabled.");

            return Result.Success();
        }

        public async Task<Result> EnableProductAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Patch,
                $"api/Admin/products/{productId}/enable"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Product could not be enabled.");

            return Result.Success();
        }

        public async Task<Result<List<OrderListDto>>> GetAdminOrdersAsync()
        {
            
            var response = await SendAsync(
                HttpMethod.Get,
                "api/Admin/orders"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Orders could not be loaded.");

            var orders =
                await response.Content.ReadFromJsonAsync<List<OrderListDto>>();

            return Result.Success(orders ?? new());
        }

        public async Task<Result<List<ActiveSellerDto>>> GetActiveSellersAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/Admin/active-sellers"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Active sellers could not be loaded.");

            var data =
                await response.Content.ReadFromJsonAsync<List<ActiveSellerDto>>();

            return Result.Success(data ?? new());
        }

        public async Task<Result<TotalEarningDto>> GetTotalEarningsAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/Admin/total-earnings"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Total earnings could not be loaded.");

            var data =
                await response.Content.ReadFromJsonAsync<TotalEarningDto>();

            return Result.Success(data ?? new());
        }

        public async Task<Result<List<AdminContactMessageDto>>> GetContactMessagesAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/Admin/contacts"
            );

            if (!response.IsSuccessStatusCode)
                return Result.Error("Contact messages could not be loaded.");

            var messages =
                await response.Content.ReadFromJsonAsync<List<AdminContactMessageDto>>();

            return Result.Success(messages ?? new());
        }

        public async Task<Result<AdminContactMessageDto>> GetContactByIdAsync(int id)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"api/admin/contacts/{id}"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Contact message could not be loaded.");

            var data = await response.Content
                .ReadFromJsonAsync<AdminContactMessageDto>();

            return Result.Success(data!);
        }

        public async Task<Result> DeleteContactAsync(int id)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/admin/contacts/{id}"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Contact message could not be deleted.");

            return Result.Success();
        }
    }
}
