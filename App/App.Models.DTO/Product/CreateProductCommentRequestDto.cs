namespace App.Models.DTO.Product
{
    public class CreateProductCommentRequestDto
    {
        public byte StarCount { get; set; }
        public string Text { get; set; } = null!;
    }
}
