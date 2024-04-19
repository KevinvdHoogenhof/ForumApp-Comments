using CommentService.API;
using CommentService.API.Context;
using CommentService.API.SeedData;
using CommentService.API.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommentService.Tests
{
    public class CommentServiceTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        private readonly ICommentService _service;

        public CommentServiceTests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var dataSeedingConfig = new DataSeedingConfiguration { SeedDataEnabled = false };
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
                        services.RemoveAll<IDataSeedingConfiguration>();
                        services.AddSingleton<IDataSeedingConfiguration>(dataSeedingConfig);
                    });
                });
            _client = appFactory.CreateClient();
            _service = new API.Services.CommentService(new CommentContext(_fixture.Client, dataSeedingConfig));
        }
        [Fact]
        public async Task GetCommentById_ShouldReturnCommentWithCorrectId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments"); 
            API.Models.Comment comment1 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment2 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            await collection.InsertOneAsync(comment1);
            await collection.InsertOneAsync(comment2);

            // Act
            var c1 = await _service.GetComment(comment1.Id);
            var c2 = await _service.GetComment(comment2.Id);

            // Assert
            Assert.NotNull(c1);
            Assert.Equal(comment1.Id, c1.Id);
            Assert.Equal(comment1.ThreadId, c1.ThreadId);
            Assert.Equal(comment1.ThreadName, c1.ThreadName);
            Assert.Equal(comment1.PostId, c1.PostId);
            Assert.Equal(comment1.PostName, c1.PostName);
            Assert.Equal(comment1.AuthorId, c1.AuthorId);
            Assert.Equal(comment1.AuthorName, c1.AuthorName);
            Assert.Equal(comment1.Name, c1.Name);
            Assert.Equal(comment1.Content, c1.Content);

            Assert.NotNull(c2);
            Assert.Equal(comment2.Id, c2.Id);
            Assert.Equal(comment2.ThreadId, c2.ThreadId);
            Assert.Equal(comment2.ThreadName, c2.ThreadName);
            Assert.Equal(comment2.PostId, c2.PostId);
            Assert.Equal(comment2.PostName, c2.PostName);
            Assert.Equal(comment2.AuthorId, c2.AuthorId);
            Assert.Equal(comment2.AuthorName, c2.AuthorName);
            Assert.Equal(comment2.Name, c2.Name);
            Assert.Equal(comment2.Content, c2.Content);
        }
        [Fact]
        public async Task GetAllComments_ShouldReturnComments()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            await collection.InsertOneAsync(new API.Models.Comment { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "test", Content = "test" });

            // Act
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllCommentsByName_ShouldReturnCommentsThatContainName()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment1 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment2 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment3 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "different name", Content = "test" };
            await collection.InsertOneAsync(comment1);
            await collection.InsertOneAsync(comment2);
            await collection.InsertOneAsync(comment3);

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
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment1 = new() { ThreadId = "tid", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment2 = new() { ThreadId = "tid", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment3 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "different name", Content = "test" };
            await collection.InsertOneAsync(comment1);
            await collection.InsertOneAsync(comment2);
            await collection.InsertOneAsync(comment3);

            // Act
            var comments = await _service.GetCommentsByThreadId("tid");

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("tid", resultItem.ThreadId);
        }
        [Fact]
        public async Task GetAllCommentsByPostId_ShouldReturnCommentsWithPostId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment1 = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment2 = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment3 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "different name", Content = "test" };
            await collection.InsertOneAsync(comment1);
            await collection.InsertOneAsync(comment2);
            await collection.InsertOneAsync(comment3);

            // Act
            var comments = await _service.GetCommentsByPostId("pid");

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal("pid", resultItem.PostId);
        }
        [Fact]
        public async Task GetAllCommentsByAuthorId_ShouldReturnCommentsWithAuthorId()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment1 = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment2 = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            API.Models.Comment comment3 = new() { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 1, AuthorName = "test", Name = "different name", Content = "test" };
            await collection.InsertOneAsync(comment1);
            await collection.InsertOneAsync(comment2);
            await collection.InsertOneAsync(comment3);

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
            API.Models.Comment comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };

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
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateOneComment_ShouldUpdateComment()
        {
            //Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            await collection.InsertOneAsync(comment);

            // Act
            var c = await _service.GetComment(comment.Id);
            c.Name = "Updated Name";
            c.Content = "Update Content";
            _ = await _service.UpdateComment(c);
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(c);
            Assert.Equal(comment.Id, c.Id);
            Assert.Equal("Updated Name", c.Name);
            Assert.Equal("Update Content", c.Content);

            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteOneComment_ShouldDeleteComment()
        {
            //Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            await collection.InsertOneAsync(comment);

            // Act
            await _service.DeleteComment(comment.Id);
            var comments = await _service.GetComments();

            // Assert
            Assert.NotNull(comments);
            Assert.Empty(comments);
        }
        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
