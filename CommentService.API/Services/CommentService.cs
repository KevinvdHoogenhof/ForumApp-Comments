using CommentService.API.Context;
using CommentService.API.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Xml.Linq;

namespace CommentService.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentContext _context;
        public CommentService(ICommentContext context)
        {
            _context = context;
        }
        public async Task<Comment?> GetComment(string id)
        {
            return await _context.GetAsync(id);
        }

        public async Task<List<Comment>> GetComments()
        {
            return await _context.GetAsync();
        }

        public async Task InsertComment(Comment comment)
        {
            await _context.CreateAsync(comment);
        }

        public async Task UpdateComment(Comment comment)
        {
            await _context.UpdateAsync(comment);
        }

        public async Task DeleteComment(string id)
        {
            await _context.RemoveAsync(id);
        }

        /*private readonly IMongoCollection<Comment> _comments;
        public CommentService(IOptions<CommentDBSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _comments = mongoDatabase.GetCollection<Comment>(settings.Value.CollectionName);
        }
        public async Task<Comment?> GetComment(string id)
        {
            return await _comments.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetComments()
        {
            return await _comments.Find(_ => true).ToListAsync();
        }

        public async Task InsertComment(Comment comment)
        {
            await _comments.InsertOneAsync(comment);
        }

        public async Task UpdateComment(Comment comment)
        {
            await _comments.ReplaceOneAsync(x => x.Id == comment.Id, comment);
        }

        public async Task DeleteComment(string id)
        {
            await _comments.DeleteOneAsync(x => x.Id == id);
        }*/
    }
}
