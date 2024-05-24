using CommentService.API.Services;
using Confluent.Kafka;
using System.Text.Json;

namespace CommentService.API.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly ILogger<KafkaConsumer> _log;
        private readonly IConsumer<Null, string> _consumer;
        private readonly ICommentService _service;

        public KafkaConsumer(ILogger<KafkaConsumer> log, IConsumer<Null, string> consumer, ICommentService service)
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
                    _consumer.Subscribe("accountdeleted");

                    var consumeResult = _consumer.Consume(stoppingToken);
                    var mv = consumeResult.Message.Value;
                    _log.LogInformation(mv);

                    try
                    {
                        var accountId = 0;
                        var parts = mv.Split(" ");
                        if (parts.Length > 0)
                        {
                            accountId = JsonSerializer.Deserialize<int>(parts[0]);
                            //_log.LogInformation(accountId.ToString());
                            var comments = accountId != 0 ? await _service.GetCommentsByAuthorId(accountId) : null;
                            foreach (var comment in comments)
                            {
                                await _service.DeleteComment(comment.Id);
                            }
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
    }
}