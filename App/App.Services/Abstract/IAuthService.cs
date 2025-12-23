using App.Models.DTO.Auth;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IAuthService
    {
        Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
        Task<Result> RegisterAsync(RegisterRequestDto dto);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto dto);
        Task<Result> RenewPasswordAsync(RenewPasswordRequestDto dto);
    }
}
