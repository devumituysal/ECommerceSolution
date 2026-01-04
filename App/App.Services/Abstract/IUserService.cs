using App.Models.DTO.User;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IUserService
    {
        Task<Result<List<UserListItemDto>>> GetUsersAsync();
        Task<Result> ApproveAsync(int userId);
        Task<Result> EnableAsync(int userId);
        Task<Result> DisableAsync(int userId);


    }
}
