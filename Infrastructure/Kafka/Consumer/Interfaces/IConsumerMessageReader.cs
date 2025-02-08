using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer;

public interface IConsumerMessageReader<TKey, TValue>
{
    public Task ReadAsync(ChannelWriter<KeyValuePair<TKey, TValue>> channelWriter, CancellationToken cancellationToken);
}