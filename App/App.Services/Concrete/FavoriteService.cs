using App.Models.DTO.Favorite;
using App.Services.Abstract;
using App.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class FavoriteService : BaseService , IFavoriteService
    {
        public FavoriteService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {
            
        }

        public async Task<bool> AddAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"/api/favorites/{productId}",
                jwt
            );

            if(response.IsSuccessStatusCode)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Delete,
                $"/api/favorites/{productId}",
                jwt
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> IsFavoriteAsync(string jwt, int productId)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"/api/favorites/check/{productId}",
                jwt
            );

            if (!response.IsSuccessStatusCode)
                return false;

            return await response.Content.ReadFromJsonAsync<bool>();
        }


        public async Task<List<MostFavoritedProductDto>> GetMostFavoritedAsync(int take)
        {
            var response = await DataClient.GetFromJsonAsync<List<MostFavoritedProductDto>>(
                $"/api/favorites/most-favorited?take={take}"
            );

            return response ?? new();
        }
    }
}
