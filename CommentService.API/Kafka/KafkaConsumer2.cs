using CommentService.API.Services;
using Confluent.Kafka;
using System.Text.Json;

namespace CommentService.API.Kafka
{
    public class KafkaConsumer2 : BackgroundService
    {
        private readonly ILogger<KafkaConsumer2> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly ICommentService _service;

        public KafkaConsumer2(ILogger<KafkaConsumer2> log, IConsumer<Null, string> consumer, ICommentService service)
        {
            _log = log;
            _consumer = consumer;
            _service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            var i = 0;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _consumer.Subscribe("updatepostname");

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var mv = consumeResult.Message.Value;
                    _log.LogInformation(mv);

                    try
                    {
                        var t = JsonSerializer.Deserialize<PostIdName>(mv);
                        var comments = t != null ? await _service.GetCommentsByPostId(t.Id) : null;
                        foreach (var comment in comments)
                        {
                            comment.PostName = t?.Name;
                            await _service.UpdateComment(comment);
                        }

                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON deserialization failed: {ex.Message}");
                    }

                    //if (i++ % 1000 == 0)
                    //{
                        _consumer.Commit();
                    //}
                }
                catch (ConsumeException ex)
                {
                    _log.LogInformation($"Error consuming message: {ex.Error.Reason}");

                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "An unexpected error occurred while consuming messages.");
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
        private class PostIdName
        {
            public string Id { get; set; } = null!;
            public string Name { get; set; } = null!;
        }
    }
}