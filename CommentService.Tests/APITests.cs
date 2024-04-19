using CommentService.API;
using CommentService.API.SeedData;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommentService.Tests
{
    public class APITests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        public APITests(MongoDbFixture fixture)
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
        }
        [Fact]
        public async Task GetAllComments_ShouldReturnComments()
        {
            // Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            await collection.InsertOneAsync(new API.Models.Comment { ThreadId = "test", ThreadName = "test", PostId = "test", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "test", Content = "test" });

            // Act
            var res = await _client.GetAsync("/comment");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task GetAllComments_NoCommentsInDb_ShouldReturnNoComments()
        {
            // Act
            var res = await _client.GetAsync("/comment");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(comments);
            Assert.Empty(comments);
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
            var res = await _client.GetAsync("/comment/getcommentsbyname?name=Test");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

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
            var res = await _client.GetAsync("/comment/getcommentsbythreadid?id=tid");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("tid", resultItem.ThreadId);
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
            var res = await _client.GetAsync("/comment/getcommentsbypostid?id=pid");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Contains("pid", resultItem.PostId);
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
            var res = await _client.GetAsync("/comment/getcommentsbyauthorid?id=0");
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Equal(2, comments.Count);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
            Assert.Equal(0, resultItem.AuthorId);
        }
        [Fact]
        public async Task PostComment_ShouldPostComment()
        {
            //Arrange
            API.Models.InsertCommentDTO comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };

            // Act
            var res = await _client.PostAsync("/comment/", new StringContent(JsonSerializer.Serialize(comment), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var c = JsonSerializer.Deserialize<API.Models.Comment>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/comment");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(c);
            Assert.Equal(comment.Name, c.Name);
            Assert.Equal(comment.Content, c.Content);
            Assert.Equal(comment.ThreadId, c.ThreadId);
            Assert.Equal(comment.PostId, c.PostId);

            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task UpdateComment_ShouldUpdateComment()
        {
            //Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            await collection.InsertOneAsync(comment);

            API.Models.UpdateCommentDTO updatedcomment = new() { Name = "Updated test name", Content = "Updated content" };

            // Act
            var res = await _client.PutAsync($"/comment/{comment.Id}", new StringContent(JsonSerializer.Serialize(updatedcomment), Encoding.UTF8, "application/json"));
            res.EnsureSuccessStatusCode();
            var content = await res.Content.ReadAsStringAsync();
            var c = JsonSerializer.Deserialize<API.Models.Comment>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            var res2 = await _client.GetAsync("/comment");
            res2.EnsureSuccessStatusCode();
            var content2 = await res2.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content2, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            // Assert
            Assert.NotNull(c);
            Assert.Equal(updatedcomment.Name, c.Name);
            Assert.Equal(updatedcomment.Content, c.Content);
            Assert.Equal(comment.ThreadId, c.ThreadId);
            Assert.Equal(comment.PostId, c.PostId);

            Assert.NotNull(comments);
            Assert.NotEmpty(comments);
            Assert.Single(comments);
            var resultItem = comments.FirstOrDefault();
            Assert.NotNull(resultItem);
        }
        [Fact]
        public async Task DeleteComment_ShouldDeleteComment()
        {
            //Arrange
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            API.Models.Comment comment = new() { ThreadId = "tid", ThreadName = "test", PostId = "pid", PostName = "test", AuthorId = 0, AuthorName = "test", Name = "Test", Content = "test" };
            await collection.InsertOneAsync(comment);

            // Act
            var res = await _client.DeleteAsync($"/comment/{comment.Id}");
            res.EnsureSuccessStatusCode();

            var res2 = await _client.GetAsync("/comment");
            res2.EnsureSuccessStatusCode();
            var content = await res2.Content.ReadAsStringAsync();
            var comments = JsonSerializer.Deserialize<ICollection<API.Models.Comment>>(content, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

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
