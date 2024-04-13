using CommentService.API.Models;

namespace CommentService.API.Context
{
    public interface ICommentContext
    {
        public Task<Comment?> GetAsync(string id);
        public Task<List<Comment>> GetAsync();
        public Task CreateAsync(Comment comment);
        public Task UpdateAsync(Comment comment);
        public Task RemoveAsync(string id);
    }
}
