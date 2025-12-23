namespace App.Models.DTO.Auth
{
    public class RenewPasswordRequestDto
    {
        public string Token { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
