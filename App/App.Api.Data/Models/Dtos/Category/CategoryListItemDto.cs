namespace App.Api.Data.Models.Dtos.Category
{
    public class CategoryListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string IconCssClass { get; set; } = null!;
    }
}
