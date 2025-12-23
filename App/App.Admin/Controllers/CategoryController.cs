using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Category;
using App.Services.Abstract;
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
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _categoryService.GetAllAsync();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Categories could not be loaded.";
                return View(new List<CategoryListViewModel>());
            }

            var categories = result.Value
                .Select(c => new CategoryListViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Color = c.Color,
                    IconCssClass = c.IconCssClass
                })
                .ToList();

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
                return View(newCategoryModel);

            var dto = new SaveCategoryDto
            {
                Name = newCategoryModel.Name,
                Color = newCategoryModel.Color,
                IconCssClass = newCategoryModel.IconCssClass
            };

            var result = await _categoryService.CreateAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Category could not be created.";
                return View(newCategoryModel);
            }

            TempData["SuccessMessage"] = "Category created successfully.";
            ModelState.Clear();

            return View();
        }

        [Route("{categoryId:int}/edit")]
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int categoryId)
        {
            var result = await _categoryService.GetAllAsync();

            if (!result.IsSuccess)
                return NotFound();

            var category = result.Value.FirstOrDefault(c => c.Id == categoryId);

            if (category == null)
                return NotFound();

            var model = new SaveCategoryViewModel
            {
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            };

            return View(model);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int categoryId, [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
                return View(editCategoryModel);

            var dto = new SaveCategoryDto
            {
                Name = editCategoryModel.Name,
                Color = editCategoryModel.Color,
                IconCssClass = editCategoryModel.IconCssClass
            };

            var result = await _categoryService.UpdateAsync(categoryId, dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = "Category could not be updated.";
                return View(editCategoryModel);
            }

            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(List));
        }

        [Route("{categoryId:int}/delete")]
        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
            var result = await _categoryService.DeleteAsync(categoryId);

            if (!result.IsSuccess)
                TempData["ErrorMessage"] = "Category could not be deleted.";
            else
                TempData["SuccessMessage"] = "Category deleted successfully.";

            return RedirectToAction(nameof(List));
        }

    }
}
