using App.Admin.Models.ViewModels;
using App.Data.Contexts;
using App.Data.Entities;
using App.Data.Repositories.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace App.Admin.Controllers
{
    [Route("/categories")]
    [Authorize(Roles = "admin")]
    public class CategoryController : BaseController
    {
        public CategoryController(HttpClient httpClient) : base(httpClient) { }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:5001/api/categories");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Categories could not be loaded.";
                return View(new List<CategoryListViewModel>());
            }

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryListViewModel>>();
            categories ??= new List<CategoryListViewModel>();

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

            SetJwtHeader();

            var response = await _httpClient.PostAsJsonAsync(
                "https://localhost:5001/api/categories",
                new
                {
                    Name = newCategoryModel.Name,
                    Color = newCategoryModel.Color,
                    IconCssClass = newCategoryModel.IconCssClass
                });

            if (!response.IsSuccessStatusCode)
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
            SetJwtHeader();

            var response = await _httpClient.GetAsync("https://localhost:5001/api/categories");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryListViewModel>>();
            var category = categories?.FirstOrDefault(c => c.Id == categoryId);
            if (category == null)
                return NotFound();

            var editModel = new SaveCategoryViewModel
            {
                Name = category.Name,
                Color = category.Color,
                IconCssClass = category.IconCssClass
            };

            return View(editModel);
        }

        [Route("{categoryId:int}/edit")]
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] int categoryId, [FromForm] SaveCategoryViewModel editCategoryModel)
        {
            if (!ModelState.IsValid)
                return View(editCategoryModel);

            SetJwtHeader();

            var response = await _httpClient.PutAsJsonAsync(
                $"https://localhost:5001/api/categories/{categoryId}",
                new
                {
                    Name = editCategoryModel.Name,
                    Color = editCategoryModel.Color,
                    IconCssClass = editCategoryModel.IconCssClass
                });

            if (!response.IsSuccessStatusCode)
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
            SetJwtHeader();

            var response = await _httpClient.DeleteAsync($"https://localhost:5001/api/categories/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Category could not be deleted.";
            }

            return RedirectToAction(nameof(List));
        }

    }
}
