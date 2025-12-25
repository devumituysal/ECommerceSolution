using App.Models.DTO.Auth;
using App.Models.DTO.Profile;
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
    public class AuthService : BaseService , IAuthService
    {
        public AuthService(IHttpClientFactory factory)
            : base(factory)
        {
        }

        // POST /api/auth/login
        public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/login",
                dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound("Email veya şifre hatalı");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Login failed");

            var result =
                await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            return Result.Success(result!);
        }

        // POST /api/auth/register
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

        // POST /api/auth/forgot-password
        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/forgot-password",
                dto);

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // POST /api/auth/renew-password
        public async Task<Result> RenewPasswordAsync(RenewPasswordRequestDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/auth/renew-password",
                dto);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result<ProfileDetailDto>> GetProfileAsync(string token)
        {
            DataClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await DataClient.GetAsync("api/users/profile");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            var result = await response.Content.ReadFromJsonAsync<ProfileDetailDto>();

            return Result.Success(result!);
        }
    }
}
