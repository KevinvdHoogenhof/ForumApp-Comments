using CommentService.API.Models;

namespace CommentService.API.Services
{
    public interface ICommentService
    {
        public Task<Comment?> GetComment(string id);
        public Task<List<Comment>> GetComments();
        public Task<List<Comment>> GetCommentsByName(string name);
        public Task<List<Comment>> GetCommentsByThreadId(string id);
        public Task<List<Comment>> GetCommentsByPostId(string id);
        public Task<int> GetAmountOfCommentsByPostId(string id);
        public Task<List<Comment>> GetCommentsByAuthorId(int id);
        public Task<Comment?> InsertComment(Comment comment);
        public Task<Comment?> UpdateComment(Comment comment);
        public Task DeleteComment(string id);
    }
}
