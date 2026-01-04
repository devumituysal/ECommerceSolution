using App.Models.DTO.Favorite;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class FavoriteService : BaseService , IFavoriteService
    {
        public FavoriteService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor) : base(httpClientFactory, httpContextAccessor)
        {
            
        }

        public async Task<bool> AddAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"/api/favorites/{productId}"
            );

            if(response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"/api/favorites/{productId}"
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsFavoriteAsync(int productId)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"/api/favorites/check/{productId}"
            );

            if (!response.IsSuccessStatusCode)
                return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }


        public async Task<List<MostFavoritedProductDto>> GetMostFavoritedAsync(int take)
        {
            var response = await DataClient.GetAsync($"/api/favorites/most-favorited?take={take}");

            if (!response.IsSuccessStatusCode)
                return new List<MostFavoritedProductDto>();

            var items = await response.Content
                .ReadFromJsonAsync<List<MostFavoritedProductDto>>();

            return items ?? new List<MostFavoritedProductDto>();
        }

        public async Task<Result<List<MyFavoriteProductDto>>> GetMyFavoritesAsync()
        {
            var response = await SendAsync( 
                HttpMethod.Get,
                "/api/favorites/my"
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Favoriler alınamadı");

            var items = await response.Content
                .ReadFromJsonAsync<List<MyFavoriteProductDto>>();

            return Result.Success(items ?? new List<MyFavoriteProductDto>());
        }
    }
}
