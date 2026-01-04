using App.Models.DTO.Cart;
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
    public class CartService : BaseService , ICartService
    {
        public CartService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {

        }

        public async Task<Result<List<CartItemDto>>> GetMyCartAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/cart"
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Sepet bilgileri alınamadı");

            var items = await response.Content
                .ReadFromJsonAsync<List<CartItemDto>>();

            return Result.Success(items ?? new List<CartItemDto>());
        }

        public async Task<Result> AddProductAsync(int productId, byte? quantity)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/cart/add/{productId}?quantity={quantity}"
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Ürün bulunamadı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Ürün sepete eklenemedi");

            return Result.Success();
        }


        public async Task<Result> UpdateItemAsync(
            int cartItemId,
            UpdateCartItemDto dto)
        {
            var response = await SendAsync(
                HttpMethod.Put,
                $"api/cart/{cartItemId}",
                dto
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Sepet ürünü bulunamadı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Sepet güncellenemedi");

            return Result.Success();
        }

        public async Task<Result> RemoveItemAsync(int cartItemId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/cart/{cartItemId}"
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Sepet ürünü bulunamadı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Ürün sepetten silinemedi");

            return Result.Success();
        }

        public async Task<Result> CheckoutAsync()
        {
            var response = await SendAsync(
                HttpMethod.Post,
                "api/cart/checkout"
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Checkout failed.");

            return Result.Success();
        }
    }
}
