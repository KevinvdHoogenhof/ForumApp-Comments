﻿using CommentService.API.Kafka;
using CommentService.API.Models;
using CommentService.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace CommentService.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        private readonly IKafkaProducer _producer;
        public CommentController(ICommentService service, IKafkaProducer producer)
        {
            _service = service;
            _producer = producer;
        }

        [HttpGet]
        public async Task<List<Comment>> Get() =>
            await _service.GetComments();

        [HttpGet("GetCommentsByName/{name}")]
        public async Task<List<Comment>> GetCommentsByName(string name) =>
            await _service.GetCommentsByName(name);

        [HttpGet("GetCommentsByThreadId/{id:length(24)}")]
        public async Task<List<Comment>> GetCommentsByThreadId(string id) =>
            await _service.GetCommentsByThreadId(id);

        [HttpGet("GetCommentsByPostId/{id:length(24)}")]
        public async Task<List<Comment>> GetCommentsByPostId(string id) =>
            await _service.GetCommentsByPostId(id);

        [HttpGet("GetCommentsByAuthorId/{id)}")]
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
        public async Task<ActionResult<Comment>> Post(InsertCommentDTO comment, CancellationToken stoppingToken)
        {
            var insertedComment = await _service.InsertComment(new Comment { ThreadId = comment.ThreadId, ThreadName = comment.ThreadName, PostId = comment.PostId, PostName = comment.PostName, AuthorId = comment.AuthorId, AuthorName = comment.AuthorName, Name = comment.Name, Content = comment.Content });

            int comments = await _service.GetAmountOfCommentsByPostId(insertedComment.PostId);

            _ = _producer.Produce(JsonSerializer.Serialize(new { insertedComment?.PostId, comments }), stoppingToken);

            return CreatedAtAction(nameof(Get), new
            {
                id = (insertedComment?.Id) ?? throw new InvalidOperationException("Failed to insert the comment.")
            }, comment);
        }

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
        public async Task<IActionResult> Delete(string id, CancellationToken stoppingToken)
        {
            var c = await _service.GetComment(id);

            if (c is null)
            {
                return NotFound();
            }

            await _service.DeleteComment(id);

            int comments = await _service.GetAmountOfCommentsByPostId(c.PostId);

            _ = _producer.Produce(JsonSerializer.Serialize(new { c?.PostId, comments }), stoppingToken);

            return NoContent();
        }
    }
}
