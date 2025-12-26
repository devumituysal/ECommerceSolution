using App.Models.DTO.Order;
using App.Models.DTO.Product;
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
        public List<OrderDto> Orders { get; set; } = new();
        public List<ProductListItemDto> Products { get; set; } = new();
        public bool HasSellerRequest { get; set; }
    }
}
