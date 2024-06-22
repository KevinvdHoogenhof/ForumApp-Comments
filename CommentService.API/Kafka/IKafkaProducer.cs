namespace CommentService.API.Kafka
{
    public interface IKafkaProducer
    {
        Task Produce(string message, CancellationToken cancellationToken);
        Task ProduceMultiple(IReadOnlyCollection<string> messages, CancellationToken cancellationToken);
    }
}
