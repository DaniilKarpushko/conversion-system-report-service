namespace KafkaInfrastructure.Consumer;

public interface IMessageHandler<TKey, TValue>
{
    public Task HandleBatchesAsync(
        IAsyncEnumerable<IReadOnlyList<KeyValuePair<TKey, TValue>>> messageBatches,
        CancellationToken cancellationToken);
}