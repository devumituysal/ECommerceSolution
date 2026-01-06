using App.Models.DTO.User;
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
    public class UserService : BaseService, IUserService
    {
        public UserService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory, httpContextAccessor)
        {

        }
        
        public async Task<Result<List<UserListItemDto>>> GetUsersAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/user/list"
                );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var users =
                await response.Content.ReadFromJsonAsync<List<UserListItemDto>>();

            return Result.Success(users!);
        }

       
        public async Task<Result> ApproveAsync(int userId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/user/{userId}/approve"
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result> RevokeSellerAsync(int userId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/user/{userId}/revoke-seller"
            );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result> EnableAsync(int userId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/user/{userId}/enable"
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }


        
        public async Task<Result> DisableAsync(int userId)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                $"api/user/{userId}/disable"
                );

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

    }
}
