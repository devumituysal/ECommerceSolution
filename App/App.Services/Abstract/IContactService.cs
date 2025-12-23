using App.Models.DTO.Contact;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface IContactService
    {
        Task<Result> SendMessageAsync(CreateContactMessageDto dto);
    }
}
