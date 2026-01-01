using App.Models.DTO.Category;
using App.Models.DTO.Favorite;
using App.Models.DTO.Product;

namespace App.Eticaret.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<CategoryWithImageDto> Categories { get; set; } = new();
        public List<ProductListItemDto> LatestProducts { get; set; } = new();
        public List<MostFavoritedProductDto> FeaturedProducts { get; set; } = new();

    }
}
