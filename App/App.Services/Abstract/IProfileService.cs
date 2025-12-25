using App.Models.DTO.Profile;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IProfileService
    {
        Task<Result<ProfileDetailDto>> GetMyProfileAsync(string jwt);
        Task<Result> UpdateMyProfileAsync(string jwt, UpdateProfileDto dto);
    }
}
