using App.Api.Data.Models.Dtos.Category;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Api.Data.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IDataRepository _repo;

        public CategoriesController(IDataRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repo.GetAll<CategoryEntity>()
                .Select(c => new CategoryListItemDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}
