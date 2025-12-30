using App.Models.DTO.Category;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

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
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
          
            var categories = await _repo.GetAll<CategoryEntity>()
                .Select(c => new CategoryListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    IconCssClass = c.IconCssClass
                })
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] SaveCategoryDto newCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryEntity = new CategoryEntity
            {
                Name = newCategory.Name,
                Color = newCategory.Color,
                IconCssClass = newCategory.IconCssClass
            };

            await _repo.Add(categoryEntity);

            return Ok(new { message = "Category created successfully." });
        }

        [HttpPut("{categoryId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int categoryId, [FromBody] SaveCategoryDto editCategory)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _repo.GetAll<CategoryEntity>()
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var nameExists = await _repo.GetAll<CategoryEntity>()
                .AnyAsync(c => c.Id != categoryId && c.Name == editCategory.Name);

            if (nameExists)
                return Conflict(new { message = "Category name already exists." });

            category.Name = editCategory.Name;
            category.Color = editCategory.Color;
            category.IconCssClass = editCategory.IconCssClass ?? "";

            await _repo.Update(category);

            return Ok();
        }
    

        [HttpDelete("{categoryId:int}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int categoryId)
        {
            var hasProducts = await _repo.GetAll<ProductEntity>()
               .AnyAsync(p => p.CategoryId == categoryId);

            if (hasProducts)
                return Conflict(new
                {
                    message = "Category has products and cannot be deleted."
                });

            var category = await _repo.GetAll<CategoryEntity>()
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            await _repo.Delete(category);

            return NoContent();
        }


        [HttpGet("with-first-product")]
        public async Task<IActionResult> GetCategoriesWithFirstProduct()
        {
            var categories = await _repo.GetAll<CategoryEntity>()
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    FirstProductImage = _repo.GetAll<ProductEntity>()
                        .Where(p => p.CategoryId == c.Id)
                        .OrderBy(p => p.Id)
                        .SelectMany(p => p.Images)
                        .Select(i => i.Url)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(categories);
        }
    }
}
