namespace KafkaInfrastructure.Producer;

public interface IKafkaProducer<TKey, TValue>
{
    public Task SendMessageAsync(TKey key, TValue value, CancellationToken cancellationToken);
}