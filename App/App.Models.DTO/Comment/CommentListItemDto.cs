namespace App.Models.DTO.Comment
{
    public class CommentListItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public int StarCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
