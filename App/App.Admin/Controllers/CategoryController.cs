using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace App.Admin.Controllers
{
    [Route("/categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _dbContext;

        public CategoryController(AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    IconCssClass = c.IconCssClass
                })
                .ToListAsync();

            return View(categories);
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SaveCategoryViewModel newCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(newCategoryModel);
            }

            var categoryEntity = new CategoryEntity
            {
                Name = newCategoryModel.Name,
                Color = newCategoryModel.Color,
                IconCssClass = string.Empty,
            };

            await _dbContext.Categories.AddAsync(categoryEntity);

            ViewBag.SuccessMessage = "Kategori başarıyla oluşturuldu.";
            ModelState.Clear();

            return View();
        }

        [Route("{categoryId:int}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }

            var editCategoryModel = new SaveCategoryViewModel
            {
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            };

            return View(editCategoryModel);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int categoryId, [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
            {
                return View(editCategoryModel);
            }

            var category = await _dbContext.Categories.FirstOrDefaultAsync(c=>c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }

            category.Name = editCategoryModel.Name;
            category.Color = editCategoryModel.Color;

            await _dbContext.SaveChangesAsync();

            ViewBag.SuccessMessage = "Kategori başarıyla güncellendi.";
            ModelState.Clear();

            return View();
        }

        [Route("{categoryId:int}/delete")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
            {
                return NotFound();
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();    

            return RedirectToAction(nameof(List));
        }

    }
}
