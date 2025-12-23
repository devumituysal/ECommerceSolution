using App.Models.DTO.Cart;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ICartService
    {
        Task<Result<List<CartItemDto>>> GetMyCartAsync(string jwt);

        Task<Result> AddProductAsync(string jwt, int productId);

        Task<Result> UpdateItemAsync(string jwt, int cartItemId, UpdateCartItemDto dto);

        Task<Result> RemoveItemAsync(string jwt, int cartItemId);
        Task<Result> CheckoutAsync(string jwt);
    }
}
