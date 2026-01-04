using App.Models.DTO.Favorite;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IFavoriteService 
    {
        Task<bool> AddAsync(int productId);
        Task<bool> RemoveAsync(int productId);
        Task<bool> IsFavoriteAsync(int productId);
        Task<List<MostFavoritedProductDto>> GetMostFavoritedAsync(int take);
        Task<Result<List<MyFavoriteProductDto>>> GetMyFavoritesAsync();
    }
}
