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
        Task<Result<List<UserListItemDto>>> GetUsersAsync(string jwt);
        Task<Result> ApproveAsync(string jwt, int userId);
    }
}
