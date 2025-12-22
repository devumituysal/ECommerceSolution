using Ardalis.Result;
using App.Models.DTO.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IOrderService
    {
        Task<Result<string>> CreateAsync(string jwt,CreateOrderRequestDto request);
        Task<Result<List<OrderDto>>> GetMyOrdersAsync(string jwt);
        Task<Result<OrderDetailsResponseDto>> GetOrderDetailsAsync(string jwt,string orderCode);
    }
}
