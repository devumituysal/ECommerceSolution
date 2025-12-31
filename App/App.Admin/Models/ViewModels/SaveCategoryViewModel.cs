using System.ComponentModel.DataAnnotations;

namespace App.Admin.Models.ViewModels
{
    public class SaveCategoryViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public string Color { get; set; } = null!;

        public string IconCssClass { get; set; } = string.Empty;
    }
}
