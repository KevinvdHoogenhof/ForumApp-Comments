
using CommentService.API.Context;
using CommentService.API.Models;
using CommentService.API.Services;

namespace CommentService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //Database
            builder.Services.Configure<CommentDBSettings>(
            builder.Configuration.GetSection("CommentDB"));

            builder.Services.AddSingleton<ICommentContext, CommentContext>();

            builder.Services.AddSingleton<ICommentService, Services.CommentService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
