using CommentService.API.Context;
using CommentService.API.Kafka;
using CommentService.API.Models;
using CommentService.API.SeedData;
using CommentService.API.Services;
using Confluent.Kafka;
using MongoDB.Driver;
using Prometheus;

namespace CommentService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMetrics();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Database
            //builder.Services.Configure<CommentDBSettings>(
            //builder.Configuration.GetSection("CommentDB"));

            var connString = builder.Configuration.GetConnectionString("MongoDB");
            builder.Services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connString));

            builder.Services.AddSingleton<IDataSeedingConfiguration, DataSeedingConfiguration>();

            builder.Services.AddSingleton<ICommentContext, CommentContext>();

            builder.Services.AddSingleton<ICommentService, Services.CommentService>();

            //Kafka
            //Kafka producer
            var producerConfig = builder.Configuration.GetSection("ProducerConfig").Get<ProducerConfig>();
            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();
            builder.Services.AddSingleton<IKafkaProducer>(_ => new KafkaProducer(producer, "newcomment"));

            //Kafka consumer
            var consumerConfig = builder.Configuration.GetSection("ConsumerConfig").Get<ConsumerConfig>();
            var consumer = new ConsumerBuilder<Null, string>(consumerConfig).Build();

            builder.Services.AddHostedService(sp =>
                new KafkaConsumer(sp.GetRequiredService<ILogger<KafkaConsumer>>(), consumer, sp.GetRequiredService<ICommentService>()));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseHttpMetrics();
            app.UseMetricServer();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
