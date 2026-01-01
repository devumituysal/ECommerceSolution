using App.Models.DTO.Category;
using Ardalis.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Services.Abstract
{
    public interface ICategoryService
    {
        Task<Result<List<CategoryListItemDto>>> GetAllAsync();
        Task<Result> CreateAsync(string jwt, SaveCategoryDto dto);
        Task<Result> UpdateAsync(int categoryId, string jwt, SaveCategoryDto dto);
        Task<Result> DeleteAsync(int categoryId,string jwt);
        Task<List<CategoryWithImageDto>> GetCategoriesWithFirstProductImageAsync();
    }
}
