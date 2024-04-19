namespace CommentService.API.Models
{
    public class InsertCommentDTO
    {
        public string ThreadId { get; set; } = null!;
        public string? ThreadName { get; set; }
        public string? PostId { get; set; }
        public string? PostName { get; set; } //????
        public int AuthorId { get; set; } = 0;
        public string? AuthorName { get; set; }
        public string Name { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
