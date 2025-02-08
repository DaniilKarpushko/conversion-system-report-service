namespace KafkaInfrastructure.Consumer;

public interface IMessageHandler<TKey, TValue>
{
    public Task HandleMessagesAsync(
        IAsyncEnumerable<IReadOnlyList<KeyValuePair<TKey, TValue>>> messageBatches,
        CancellationToken cancellationToken);
}