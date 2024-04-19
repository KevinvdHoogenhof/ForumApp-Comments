using CommentService.API.Models;

namespace CommentService.API.SeedData
{
    public class SeedData
    {
        public static IEnumerable<Comment> GetComments()
        {
            return
            [
                new()
                {
                    Id = "6622350dd41b2b33c74bdbd9",
                    ThreadId = "6622190fe21ab10bc3650d6b",
                    ThreadName = "General",
                    AuthorId = 0,
                    AuthorName = "asd",
                    PostId = "662235bfc3337eb9aaf2b983",
                    PostName = "First Post!",
                    Name = "Comment 1",
                    Content = "Content",
                },
                new()
                {
                    Id = "6622350dd41b2b33c74bdbda",
                    ThreadId = "6622190fe21ab10bc3650d6b",
                    ThreadName = "General",
                    AuthorId = 0,
                    AuthorName = "asd",
                    PostId = "662235bfc3337eb9aaf2b983",
                    PostName = "First Post!",
                    Name = "Comment 2",
                    Content = "Content",
                }
            ];
        }
    }
}
