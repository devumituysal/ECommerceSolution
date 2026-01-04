using App.Models.DTO.Auth;
using App.Models.DTO.Profile;
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
    public class AuthService : BaseService , IAuthService
    {
        public AuthService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor)
        {
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/login",
                dto
            );

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                var message = await response.Content.ReadAsStringAsync();
                return Result.Forbidden(message);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized("Email veya şifre hatalı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Login failed");

            var result =
                await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return Result.Success(result!);
        }

        public async Task<Result> RegisterAsync(RegisterRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/register",
                dto);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/forgot-password",
                dto);

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result> RenewPasswordAsync(RenewPasswordRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/renew-password",
                dto);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var msg = await response.Content.ReadAsStringAsync();
                return Result.Error(msg);
            }

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result<ProfileDetailDto>> GetProfileAsync()
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/users/profile"
                );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result = await response.Content.ReadFromJsonAsync<ProfileDetailDto>();

            return Result.Success(result!);
        }

     
    }
}
