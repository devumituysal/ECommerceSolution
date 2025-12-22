namespace App.Models.DTO.Auth
{
    public class LoginResponseDto
    {
        public int Id { get; set; } 

        public string Email { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Role { get; set; } = null!;

        public string Token { get; set; } = null!;
    }
}
