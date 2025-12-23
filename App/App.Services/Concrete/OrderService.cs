using App.Models.DTO.Order;
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
    public class OrderService : BaseService , IOrderService
    {
        public OrderService(IHttpClientFactory factory) : base(factory) 
        { 
        
        }


    // POST /api/order
    public async Task<Result<string>> CreateAsync(
            string jwt,
            CreateOrderRequestDto createOrderRequestDto)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                "api/order",
                jwt,
                createOrderRequestDto);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            // API: { orderCode = "XXXX" } 
            var result =
                await response.Content.ReadFromJsonAsync<CreateOrderResponseDto>();

            return Result.Success(result!.OrderCode);
        }

        // GET /api/order/my-orders
        public async Task<Result<List<OrderDto>>> GetMyOrdersAsync(
            string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/order/my-orders",
                jwt);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result =
                await response.Content.ReadFromJsonAsync<List<OrderDto>>();

            return Result.Success(result!);
        }

        // GET /api/order/{orderCode}
        public async Task<Result<OrderDetailsResponseDto>> GetOrderDetailsAsync(
            string jwt,
            string orderCode)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"api/order/{orderCode}",
                jwt);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result =
                await response.Content
                    .ReadFromJsonAsync<OrderDetailsResponseDto>();

            return Result.Success(result!);
        }

    }
}
