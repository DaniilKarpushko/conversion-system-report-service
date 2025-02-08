using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer;

public interface IConsumerMessageHandler<TKey, TValue>
{
    public Task HandleAsync(ChannelReader<KeyValuePair<TKey, TValue>> channelReader, CancellationToken cancellationToken);
}