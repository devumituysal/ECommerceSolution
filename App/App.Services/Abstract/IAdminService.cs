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
        Task<AdminNotificationDto?> GetNotificationsAsync();
        Task<Result<List<ProductListItemDto>>> GetAdminProductsAsync(int? categoryId, string? search);
        Task<Result> DeleteAsync(int productId);
        Task<Result> DisableProductAsync(int productId);
        Task<Result> EnableProductAsync(int productId);
        Task<Result<List<OrderListDto>>> GetAdminOrdersAsync();
        Task<Result<List<ActiveSellerDto>>> GetActiveSellersAsync();
        Task<Result<TotalEarningDto>> GetTotalEarningsAsync();
        Task<Result<List<AdminContactMessageDto>>> GetContactMessagesAsync();
        Task<Result<AdminContactMessageDto>> GetContactByIdAsync(int id);
        Task<Result> DeleteContactAsync(int id);

    }
}
