using System.ComponentModel.DataAnnotations;

namespace App.Eticaret.Models.ViewModels
{
    public class ProfileDetailsViewModel
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = null!;

        [Required, MaxLength(256), EmailAddress]
        public string Email { get; set; } = null!;

        // 🔹 API’den gelen mevcut dosya adı
        public string? ProfileImagePath { get; set; }

        // 🔹 Formdan upload edilen dosya
        public IFormFile? ProfileImage { get; set; }
    }
}
