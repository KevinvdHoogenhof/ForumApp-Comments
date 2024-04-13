using CommentService.API.Models;

namespace CommentService.API.Services
{
    public interface ICommentService
    {
        public Task<Comment?> GetComment(string id);
        public Task<List<Comment>> GetComments();
        public Task InsertComment(Comment comment);
        public Task UpdateComment(Comment comment);
        public Task DeleteComment(string id);
    }
}
