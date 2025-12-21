using App.Api.Data.Models.Dtos.Contact;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public ContactController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateContactMessageDto dto)
        {
            var entity = new ContactMessageEntity
            {
                Name = dto.Name,
                Email = dto.Email,
                Message = dto.Message
            };

            await _repo.Add(entity);
            return Ok();
        }
    }
}
