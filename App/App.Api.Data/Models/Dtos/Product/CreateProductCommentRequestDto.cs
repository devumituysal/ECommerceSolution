namespace App.Api.Data.Models.Dtos.Product
{
    public class CreateProductCommentRequestDto
    {
        public byte StarCount { get; set; }
        public string Text { get; set; } = null!;
    }
}
