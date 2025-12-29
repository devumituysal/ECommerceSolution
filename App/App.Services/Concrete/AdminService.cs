using App.Models.DTO.Admin;
using App.Services.Abstract;
using App.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class AdminService : BaseService , IAdminService
    {
        public AdminService(IHttpClientFactory factory) : base(factory)
        {

        }

        public async Task<AdminNotificationDto?> GetNotificationsAsync(string jwt)
        {
            var response = await SendAsync(
                HttpMethod.Get,
                "api/admin/notifications",
                jwt
            );

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content
                .ReadFromJsonAsync<AdminNotificationDto>();
        }
    }
}
