using CommentService.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace CommentService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly Services.CommentService _service;
        public CommentController(Services.CommentService service) =>
            _service = service;

        [HttpGet]
        public async Task<List<Comment>> Get() =>
            await _service.GetComments(); 

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
        public async Task<IActionResult> Post(Comment comment) 
        {
            await _service.InsertComment(comment);
            return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Comment comment)
        {
            var c = await _service.GetComment(id);

            if (c is null)
            {
                return NotFound();
            }

            await _service.UpdateComment(comment);

            return NoContent();
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
