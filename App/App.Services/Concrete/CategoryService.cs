using App.Models.DTO.Category;
using App.Services.Abstract;
using App.Services.Base;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Concrete
{
    public class CategoryService : BaseService, ICategoryService
    {
        public CategoryService(IHttpClientFactory httpClientFactory) : base(httpClientFactory)
        {

        }

        // GET /api/categories
        public async Task<Result<List<CategoryListItemDto>>> GetAllAsync()
        {
            var response = await DataClient.GetAsync("api/categories");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Categories could not be loaded");

            var categories =
                await response.Content.ReadFromJsonAsync<List<CategoryListItemDto>>();

            return Result.Success(categories ?? new());
        }

        // POST /api/categories
        public async Task<Result> CreateAsync(SaveCategoryDto dto)
        {
            var response = await DataClient.PostAsJsonAsync(
                "api/categories",
                dto);

            if (response.StatusCode == HttpStatusCode.BadRequest)
                return Result.Invalid();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // PUT /api/categories/{id}
        public async Task<Result> UpdateAsync(int categoryId, SaveCategoryDto dto)
        {
            var response = await DataClient.PutAsJsonAsync(
                $"api/categories/{categoryId}",
                dto);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        // DELETE /api/categories/{id}
        public async Task<Result> DeleteAsync(int categoryId)
        {
            var response = await DataClient.DeleteAsync(
                $"api/categories/{categoryId}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return Result.NotFound();

            if (!response.IsSuccessStatusCode)
                return Result.Error();

            return Result.Success();
        }

        public async Task<Result<List<CategoryWithImageDto>>> GetCategoriesWithFirstProductImageAsync()
        {
            var response = await DataClient.GetAsync("api/categories/with-first-product");

            if (!response.IsSuccessStatusCode)
                return Result.Error("Categories could not be loaded");

            var categories =
                await response.Content.ReadFromJsonAsync<List<CategoryWithImageDto>>();

            return Result.Success(categories ?? new());
        }

    }
}