using CommentService.API.Context;
using CommentService.API.Models;
using CommentService.API.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CommentService.Tests
{
    public class CommentServiceUnitTests
    {
        public CommentServiceUnitTests()
        {

        }
        [Fact]
        public async Task GetCommentById_ShouldReturnCommentWithCorrectId()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var c1 = await _service.GetComment("comment1");
            var c2 = await _service.GetComment("comment2");

            // Assert
            Assert.NotNull(c1);
            Assert.Equal("comment1", c1.Id);

            Assert.NotNull(c2);
            Assert.Equal("comment2", c2.Id);
        }
        [Fact]
        public async Task GetAllComments_ShouldReturnComments()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(3, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllCommentsByName_ShouldReturnCommentsThatContainName()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var comments = await _service.GetCommentsByName("Test");

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("Test", resultItem.Name);
        }
        [Fact]
        public async Task GetAllCommentsByThreadId_ShouldReturnCommentsWithThreadId()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var comments = await _service.GetCommentsByThreadId("tid1");

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("tid1", resultItem.ThreadId);
        }
        [Fact]
        public async Task GetAllCommentsByPostId_ShouldReturnCommentsWithPostId()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var comments = await _service.GetCommentsByPostId("post1id");

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("post1id", resultItem.PostId);
        }
        [Fact]
        public async Task GetAllCommentsByAuthorId_ShouldReturnCommentsWithAuthorId()
        {
            // Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var comments = await _service.GetCommentsByAuthorId(0);

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal(0, resultItem.AuthorId);
        }
        [Fact]
        public async Task InsertOneComment_ShouldInsertComment()
        {
            //Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());
            Comment comment = new() { Id = "comment4", ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };

            // Act
            var c = await _service.InsertComment(comment);
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(c);
            Assert.Equal(comment.Id, c.Id);
            Assert.Equal(comment.ThreadId, c.ThreadId);
            Assert.Equal(comment.ThreadName, c.ThreadName);
            Assert.Equal(comment.PostId, c.PostId);
            Assert.Equal(comment.PostName, c.PostName);
            Assert.Equal(comment.AuthorId, c.AuthorId);
            Assert.Equal(comment.AuthorName, c.AuthorName);
            Assert.Equal(comment.Name, c.Name);
            Assert.Equal(comment.Content, c.Content);

            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(4, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOneComment_ShouldUpdateComment()
        {
            //Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            var c = await _service.GetComment("comment3");
            c.Name = "Updated Name";
            c.Content = "Update Content";
            _ = await _service.UpdateComment(c);
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(c);
            Assert.Equal("comment3", c.Id);
            Assert.Equal("Updated Name", c.Name);
            Assert.Equal("Update Content", c.Content);

            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(3, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOneComment_ShouldDeleteComment()
        {
            //Arrange
            ICommentService _service = new API.Services.CommentService(new TestDAL());

            // Act
            await _service.DeleteComment("comment1");
            await _service.DeleteComment("comment2");
            await _service.DeleteComment("comment3");
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(comments);
            Assert.Empty(comments);
        }
        private class TestDAL : ICommentContext
        {
            readonly List<Comment> _comments = new List<Comment>
            {
            new() { Id = "comment1", ThreadId = "tid1", ThreadName = "Thread 1", PostId = "post1id", PostName = "post1", AuthorId = 0, AuthorName = "asd", Name = "Post 1", Content = "Content of post 1" },
            new() { Id = "comment2", ThreadId = "tid1", ThreadName = "Thread 2", PostId = "post1id", PostName = "post1", AuthorId = 0, AuthorName = "xyz", Name = "Post 2 Test", Content = "Content of post 2" },
            new() { Id = "comment3", ThreadId = "tid2", ThreadName = "Thread 3", PostId = "post2id", PostName = "post2", AuthorId = 1, AuthorName = "abc", Name = "Post 3 Test", Content = "Content of post 3" }
            };
            Task<Comment?> ICommentContext.CreateAsync(Comment comment)
            {
                _comments.Add(comment);
                return Task.FromResult<Comment?>(comment);
            }

            Task<Comment?> ICommentContext.GetAsync(string id)
            {
                Comment? comment = _comments.FirstOrDefault(comment => comment.Id == id);
                if (comment != null)
                {
                    return Task.FromResult<Comment?>(comment);
                }
                else
                {
                    return Task.FromResult<Comment?>(null);
                }
            }

            Task<List<Comment>> ICommentContext.GetAsync()
            {
                return Task.FromResult(_comments);
            }

            Task<List<Comment>> ICommentContext.GetAsyncByAuthorId(int id)
            {
                var comments = _comments.Where(comment => comment.AuthorId == id).ToList();
                return Task.FromResult(comments);
            }

            Task<List<Comment>> ICommentContext.GetAsyncByPostId(string id)
            {
                var comments = _comments.Where(comment => comment.PostId == id).ToList();
                return Task.FromResult(comments);
            }

            Task<List<Comment>> ICommentContext.GetAsyncByThreadId(string id)
            {
                var comments = _comments.Where(comment => comment.ThreadId == id).ToList();
                return Task.FromResult(comments);
            }

            Task<int> ICommentContext.GetAsyncCommentAmountByPostId(string id)
            {
                throw new NotImplementedException();
            }

            Task<List<Comment>> ICommentContext.GetAsyncNameSearch(string name)
            {
                var comments = _comments.Where(comment => comment.Name.Contains(name)).ToList();
                return Task.FromResult(comments);
            }

            Task ICommentContext.RemoveAsync(string id)
            {
                var commentToRemove = _comments.FirstOrDefault(comment => comment.Id == id);
                if (commentToRemove != null)
                {
                    _comments.Remove(commentToRemove);
                }
                return Task.CompletedTask;
            }

            Task<Comment?> ICommentContext.UpdateAsync(Comment comment)
            {
                Comment? existingComment = _comments.FirstOrDefault(p => p.Id == comment.Id);
                if (existingComment != null)
                {
                    existingComment.ThreadId = comment.ThreadId;
                    existingComment.ThreadName = comment.ThreadName;
                    existingComment.PostId = comment.PostId;
                    existingComment.PostName = comment.PostName;
                    existingComment.AuthorId = comment.AuthorId;
                    existingComment.AuthorName = comment.AuthorName;
                    existingComment.Name = comment.Name;
                    existingComment.Content = comment.Content;

                    return Task.FromResult<Comment?>(existingComment);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
