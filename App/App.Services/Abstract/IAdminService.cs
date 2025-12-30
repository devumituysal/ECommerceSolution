using App.Models.DTO.Admin;
using App.Models.DTO.Product;
using App.Services.Base;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IAdminService
    {
        Task<AdminNotificationDto?> GetNotificationsAsync(string jwt);
        Task<Result<List<ProductListItemDto>>> GetAdminProductsAsync(string jwt, int? categoryId, string? search);
        Task<Result> DeleteAsync(string jwt, int productId);

    }
}
