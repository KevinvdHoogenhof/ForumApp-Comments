using CommentService.API.Models;
using CommentService.API.SeedData;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Threading;
using System.Xml.Linq;

namespace CommentService.API.Context
{
    public class CommentContext : ICommentContext
    {
        private readonly IMongoCollection<Comment> _comments;
        public CommentContext(IMongoClient mongoClient, IDataSeedingConfiguration dataSeedingConfig)
        {
            var mongoDatabase = mongoClient.GetDatabase("CommentDB");
            _comments = mongoDatabase.GetCollection<Comment>("Comments");
            if (dataSeedingConfig.SeedDataEnabled && !_comments.AsQueryable().Any())
            {
                _comments.InsertManyAsync(SeedData.SeedData.GetComments());
            }
        }
        /*public CommentContext(IOptions<CommentDBSettings> commentdbsettings)
        {
            var mongoClient = new MongoClient(commentdbsettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(commentdbsettings.Value.DatabaseName);
            _comment = mongoDatabase.GetCollection<Comment>(commentdbsettings.Value.CollectionName);
        }*/
        public async Task<Comment?> GetAsync(string id)
        {
            return await _comments.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetAsync()
        {
            return await _comments.Find(_ => true).ToListAsync();
        }

        public async Task<List<Comment>> GetAsyncNameSearch(string name)
        {
            var filter = Builders<Comment>.Filter.Where(c => c.Name.Contains(name));
            return await (await _comments.FindAsync(filter)).ToListAsync();
        }

        public async Task<List<Comment>> GetAsyncByThreadId(string id)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.ThreadId, id);
            return await (await _comments.FindAsync(filter)).ToListAsync();
        }

        public async Task<List<Comment>> GetAsyncByPostId(string id)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.PostId, id);
            return await (await _comments.FindAsync(filter)).ToListAsync();
        }

        public async Task<List<Comment>> GetAsyncByAuthorId(int id)
        {
            var filter = Builders<Comment>.Filter.Eq(c => c.AuthorId, id);
            return await _comments.Find(_ => true).ToListAsync();
        }

        public async Task<Comment?> CreateAsync(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
            return comment;
        }

        public async Task<Comment?> UpdateAsync(Comment comment)
        {
            return (await _comments.ReplaceOneAsync(x => x.Id == comment.Id, comment)).IsAcknowledged
                ? await _comments.Find(x => x.Id == comment.Id).FirstOrDefaultAsync()
                : null;
        }

        public async Task RemoveAsync(string id)
        {
            await _comments.DeleteOneAsync(x => x.Id == id);
        }
    }
}
