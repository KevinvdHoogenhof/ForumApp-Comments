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

        public async Task<List<Comment>> GetCommentsByName(string id)
        {
            return await _context.GetAsyncNameSearch(id);
        }

        public async Task<List<Comment>> GetCommentsByThreadId(string id)
        {
            return await _context.GetAsyncByThreadId(id);
        }

        public async Task<List<Comment>> GetCommentsByPostId(string id)
        {
            return await _context.GetAsyncByPostId(id);
        }

        public async Task<List<Comment>> GetCommentsByAuthorId(int id)
        {
            return await _context.GetAsyncByAuthorId(id);
        }

        public async Task<Comment?> InsertComment(Comment comment)
        {
            return await _context.CreateAsync(comment);
        }

        public async Task<Comment?> UpdateComment(Comment comment)
        {
            return await _context.UpdateAsync(comment);
        }

        public async Task DeleteComment(string id)
        {
            await _context.RemoveAsync(id);
        }
    }
}
