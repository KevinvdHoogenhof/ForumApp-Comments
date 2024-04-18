using CommentService.API;
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
    public class CommentTests : IClassFixture<MongoDbFixture>, IDisposable
    {
        private readonly MongoDbFixture _fixture;
        private readonly HttpClient _client;
        public CommentTests(MongoDbFixture fixture)
        {
            _fixture = fixture;
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IMongoClient>();
                        services.AddSingleton<IMongoClient>(
                            (_) => _fixture.Client);
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
            // Arrange

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
        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
