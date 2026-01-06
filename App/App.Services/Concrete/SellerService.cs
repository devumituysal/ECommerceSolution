using App.Models.DTO.Seller;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class SellerService : BaseService, ISellerService
    {
        public SellerService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory, httpContextAccessor)
        {

        }

        public async Task<Result<SellerDetailDto>> GetSellerDetailAsync(int sellerId)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                $"api/seller/{sellerId}"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error("Satıcı bilgileri alınamadı.");

            var seller = await response.Content
                .ReadFromJsonAsync<SellerDetailDto>();

            return seller is null
                ? Result.NotFound()
                : Result.Success(seller);
        }


    }
}
