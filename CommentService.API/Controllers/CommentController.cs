using CommentService.API.Models;
using CommentService.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CommentService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        public CommentController(ICommentService service) =>
            _service = service;

        [HttpGet]
        public async Task<List<Comment>> Get() =>
            await _service.GetComments();

        [HttpGet("GetCommentsByName")]
        public async Task<List<Comment>> GetCommentsByName(string name) =>
            await _service.GetCommentsByName(name);

        [HttpGet("GetCommentsByThreadId")]
        public async Task<List<Comment>> GetCommentsByThreadId(string name) =>
            await _service.GetCommentsByThreadId(name);

        [HttpGet("GetCommentsByPostId")]
        public async Task<List<Comment>> GetCommentsByPostId(string name) =>
            await _service.GetCommentsByPostId(name);

        [HttpGet("GetCommentsByAuthorId")]
        public async Task<List<Comment>> GetCommentsByAuthorId(int id) =>
            await _service.GetCommentsByAuthorId(id);

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Comment>> Get(string id) 
        {
            var comment = await _service.GetComment(id);

            if (comment is null)
            {
                return NotFound();
            }

            return comment;
        }

        [HttpPost]
        public async Task<ActionResult<Comment>> Post(InsertCommentDTO comment) =>
            CreatedAtAction(nameof(Get), new
            {
                id = ((await _service.InsertComment(new Comment { ThreadId = comment.ThreadId, ThreadName = comment.ThreadName, PostId = comment.PostId, PostName = comment.PostName, AuthorId = comment.AuthorId, AuthorName = comment.AuthorName, Name = comment.Name, Content = comment.Content }))?.Id)
                ?? throw new InvalidOperationException("Failed to insert the comment.")
            }, comment);

        [HttpPut("{id:length(24)}")]
        public async Task<ActionResult<Comment>> Update(string id, UpdateCommentDTO comment)
        {
            var c = await _service.GetComment(id);

            if (c is null)
            {
                return NotFound();
            }

            c.Name = comment.Name;
            c.Content = comment.Content;

            var co = await _service.UpdateComment(c); 
            
            if (co is null)
            {
                return NotFound();
            }

            return co;
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var c = await _service.GetComment(id);

            if (c is null)
            {
                return NotFound();
            }

            await _service.DeleteComment(id);

            return NoContent();
        }
    }
}
