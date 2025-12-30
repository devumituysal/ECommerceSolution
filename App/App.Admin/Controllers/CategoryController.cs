using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using App.Models.DTO.Category;
using App.Services.Abstract;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace App.Admin.Controllers
{
    [Route("categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("")]
        public async Task<IActionResult> List(bool fromDelete = false)
        {

            if (!fromDelete)
            {
                TempData.Remove("SuccessMessage");
                TempData.Remove("WarningMessage");
                TempData.Remove("ErrorMessage");
            }
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
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
                return View(newCategoryModel);

            var dto = new SaveCategoryDto
            {
                Name = newCategoryModel.Name,
                Color = newCategoryModel.Color,
                IconCssClass = newCategoryModel.IconCssClass
            };

            var result = await _categoryService.CreateAsync(jwt,dto);

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
        public async Task<IActionResult> Edit(int categoryId)
        {
            var result = await _categoryService.GetAllAsync();

            if (!result.IsSuccess)
                return NotFound();

            var category = result.Value.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
                return NotFound();

            var model = new SaveCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            };

            return View(model);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        [FromRoute] int categoryId,
        [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                editCategoryModel.Id = categoryId;
                return View(editCategoryModel);
            }

            var dto = new SaveCategoryDto
            {
                Name = editCategoryModel.Name,
                Color = editCategoryModel.Color,
                IconCssClass = editCategoryModel.IconCssClass
            };

            var result = await _categoryService.UpdateAsync(categoryId, jwt, dto);

            if (result.IsSuccess) 
            {
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction(nameof(List));
            }


            if (result.Status == ResultStatus.Conflict)
            {
                TempData["WarningMessage"] =
                    "A category with the same name already exists.";
            }
            else if (result.Status == ResultStatus.NotFound)
            {
                TempData["ErrorMessage"] = "Category not found.";
            }
            else if (result.Status == ResultStatus.Invalid)
            {
                TempData["ErrorMessage"] = "Invalid category data.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category could not be updated.";
            }

            editCategoryModel.Id = categoryId;
            return View(editCategoryModel);
        }


        [Route("{categoryId:int}/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] int categoryId)
        {
           
            var jwt = Request.Cookies["access_token"];

            if (string.IsNullOrEmpty(jwt))
                return RedirectToAction("Login", "Auth");

            var result = await _categoryService.DeleteAsync(categoryId,jwt);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else if (result.Status == ResultStatus.Conflict)
            {
                TempData["WarningMessage"] =
                    "This category cannot be deleted because it contains products.";
            }
            else if (result.Status == ResultStatus.NotFound)
            {
                TempData["ErrorMessage"] = "Category not found.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category could not be deleted.";
            }

            return RedirectToAction(nameof(List) , new { fromDelete = true });
        }

    }
}
