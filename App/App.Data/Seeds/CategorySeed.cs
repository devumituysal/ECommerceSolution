using App.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Seeds
{
    public static class CategorySeed
    {
        public static List<CategoryEntity> Data => new()
        {
             new CategoryEntity { Id = 1, Name = "Electronics", Color = "Blue", IconCssClass = "fa fa-fw fa-bolt", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 2, Name = "Clothing", Color = "Red", IconCssClass = "fa fa-fw fa-shopping-bag", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 3, Name = "Home", Color = "Green", IconCssClass = "fa fa-fw fa-home", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 4, Name = "Books", Color = "Orange", IconCssClass = "fa fa-fw fa-book", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 5, Name = "Health", Color = "Purple", IconCssClass = "fa fa-fw fa-heart", CreatedAt = DateTime.Now },
             new CategoryEntity { Id = 6, Name = "Sports", Color = "Yellow", IconCssClass = "fa fa-fw fa-trophy", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 7, Name = "Toys", Color = "Pink", IconCssClass = "fa fa-fw fa-child", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 8, Name = "Automotive", Color = "Grey", IconCssClass = "fa fa-fw fa-car", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 9, Name = "Furniture", Color = "Brown", IconCssClass = "fa fa-fw fa-tree", CreatedAt = DateTime.UtcNow },
             new CategoryEntity { Id = 10, Name = "Food", Color = "Black", IconCssClass = "fa fa-fw fa-cutlery", CreatedAt = DateTime.UtcNow }
        };
    }
}
