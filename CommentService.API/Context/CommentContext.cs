using CommentService.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CommentService.API.Context
{
    public class CommentContext : ICommentContext
    {
        private readonly IMongoCollection<Comment> _comment;
        public CommentContext(IOptions<CommentDBSettings> commentdbsettings)
        {
            var mongoClient = new MongoClient(commentdbsettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(commentdbsettings.Value.DatabaseName);
            _comment = mongoDatabase.GetCollection<Comment>(commentdbsettings.Value.CollectionName);
        }
        public async Task<Comment?> GetAsync(string id)
        {
            return await _comment.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetAsync()
        {
            return await _comment.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            await _comment.InsertOneAsync(comment);
        }

        public async Task UpdateAsync(Comment comment)
        {
            await _comment.ReplaceOneAsync(x => x.Id == comment.Id, comment);
        }

        public async Task RemoveAsync(string id)
        {
            await _comment.DeleteOneAsync(x => x.Id == id);
        }
    }
}
