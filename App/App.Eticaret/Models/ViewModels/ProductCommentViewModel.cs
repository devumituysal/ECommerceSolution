namespace App.Eticaret.Models.ViewModels
{
    public class ProductCommentViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public string Content { get; set; } = null!;

        public int StarCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
