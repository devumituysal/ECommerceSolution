using App.Models.DTO.Cart;
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
    public class CartService : BaseService , ICartService
    {
        public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        public async Task<Result<List<CartItemDto>>> GetMyCartAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/cart",
                jwt
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Sepet bilgileri alınamadı");

            var items = await response.Content
                .ReadFromJsonAsync<List<CartItemDto>>();

            return Result.Success(items ?? new List<CartItemDto>());
        }

        public async Task<Result> AddProductAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/cart/{productId}",
                jwt
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
            string jwt,
            int cartItemId,
            UpdateCartItemDto dto)
        {
            var response = await SendAsync(
                HttpMethod.Put,
                $"api/cart/{cartItemId}",
                jwt,
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

        public async Task<Result> RemoveItemAsync(string jwt, int cartItemId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"api/cart/{cartItemId}",
                jwt
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Sepet ürünü bulunamadı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Ürün sepetten silinemedi");

            return Result.Success();
        }

        public async Task<Result> CheckoutAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                "api/cart/checkout",
                jwt
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Checkout failed.");

            return Result.Success();
        }
    }
}
