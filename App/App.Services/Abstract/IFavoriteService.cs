using App.Models.DTO.Favorite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IFavoriteService 
    {
        Task<bool> AddAsync(string jwt, int productId);
        Task<bool> RemoveAsync(string jwt, int productId);
        Task<bool> IsFavoriteAsync(string jwt, int productId);
        Task<List<MostFavoritedProductDto>> GetMostFavoritedAsync(int take);
    }
}
