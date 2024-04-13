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
    }
}
