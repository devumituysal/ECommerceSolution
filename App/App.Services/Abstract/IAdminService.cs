using App.Models.DTO.Admin;
using App.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IAdminService 
    {
        Task<AdminNotificationDto?> GetNotificationsAsync(string jwt);

    }
}
