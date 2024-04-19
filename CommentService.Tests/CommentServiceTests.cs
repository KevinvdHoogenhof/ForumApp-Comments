using CommentService.API;
using CommentService.API.Context;
using CommentService.API.SeedData;
using CommentService.API.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                    });
                });
            _client = appFactory.CreateClient();
            _service = new API.Services.CommentService(new CommentContext(_fixture.Client, dataSeedingConfig));
        }
        public void Dispose()
        {
            var _db = _fixture.Client.GetDatabase("CommentDB");
            var collection = _db.GetCollection<API.Models.Comment>("Comments");
            collection.DeleteManyAsync(_ => true).Wait();
        }
    }
}
