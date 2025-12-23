using App.Models.DTO.Contact;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class ContactService : BaseService , IContactService
    {
        public ContactService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<Result> SendMessageAsync(CreateContactMessageDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "/api/contact",
                dto
            );

            if (!response.IsSuccessStatusCode)
            {
                return Result.Error("Mesaj gönderilemedi.");
            }

            return Result.Success();
        }
    }
}
