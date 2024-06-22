using CommentService.API.Models;

namespace CommentService.API.Context
{
    public interface ICommentContext
    {
        public Task<Comment?> GetAsync(string id);
        public Task<List<Comment>> GetAsync();
        public Task<List<Comment>> GetAsyncNameSearch(string name);
        public Task<List<Comment>> GetAsyncByThreadId(string id);
        public Task<List<Comment>> GetAsyncByPostId(string id);
        public Task<int> GetAsyncCommentAmountByPostId(string id);
        public Task<List<Comment>> GetAsyncByAuthorId(int id);
        public Task<Comment?> CreateAsync(Comment comment);
        public Task<Comment?> UpdateAsync(Comment comment);
        public Task RemoveAsync(string id);
    }
}
