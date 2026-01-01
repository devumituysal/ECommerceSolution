using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Models.DTO.Favorite
{
    public class MostFavoritedProductDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public int FavoriteCount { get; set; }
    }
}
