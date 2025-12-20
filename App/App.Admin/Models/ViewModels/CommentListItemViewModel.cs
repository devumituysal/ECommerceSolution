namespace App.Admin.Models.ViewModels
{
    public class CommentListItemViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool Approved { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
