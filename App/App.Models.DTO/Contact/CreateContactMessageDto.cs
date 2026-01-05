using System.ComponentModel.DataAnnotations;

namespace App.Models.DTO.Contact
{
    public class CreateContactMessageDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime Created { get; set; }
    }
}
