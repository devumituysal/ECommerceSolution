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
using System.Text.Json;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class ProfileService : BaseService, IProfileService
    {
        public ProfileService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }
        public async Task<Result<ProfileDetailDto>> GetMyProfileAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/profile",
                jwt
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (!response.IsSuccessStatusCode)
                return Result.NotFound();

            var profile = await response.Content
                .ReadFromJsonAsync<ProfileDetailDto>();

            return profile is null
                ? Result.NotFound()
                : Result.Success(profile);
        }

        public async Task<Result> UpdateMyProfileAsync(string jwt, UpdateProfileDto dto)
        {
            var response = await SendAsync(
                HttpMethod.Put,
                "api/profile",
                jwt,
                dto
            );

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return Result.Unauthorized();

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                if (json.TryGetProperty("message", out var message))
                {
                    var msg = message.GetString() ?? "Profil güncellenemedi.";

                    return Result.Invalid(
                        new ValidationError(msg)
                    );
                }

                return Result.Invalid(
                    new ValidationError("Profil güncellenemedi.")
                );
            }

            if (!response.IsSuccessStatusCode)
                return Result.Error("Profil güncellenemedi.");

            return Result.Success();
        }

        public async Task<Result> RequestSellerAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Post,
                "api/profile/request-seller",
                jwt
            );

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

