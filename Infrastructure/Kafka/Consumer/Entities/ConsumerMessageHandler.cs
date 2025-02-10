using KafkaInfrastructure.Options;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer.Entities;

public class ConsumerMessageHandler<TKey, TValue> : IConsumerMessageHandler<TKey, TValue>
{
    private readonly IMessageHandler<TKey,TValue> _messageHandler;
    private IOptionsMonitor<BatchingOptions> _batchingOptionsMonitor;

    public ConsumerMessageHandler(
        IMessageHandler<TKey, TValue> messageHandler,
        IOptionsMonitor<BatchingOptions> batchingOptionsMonitor)
    {
        _messageHandler = messageHandler;
        _batchingOptionsMonitor = batchingOptionsMonitor;
    }

    public async Task HandleAsync(
        ChannelReader<KeyValuePair<TKey, TValue>> channelReader,
        CancellationToken cancellationToken)
    {
        await Task.Yield();

        while (!cancellationToken.IsCancellationRequested)
        {
            var currentBatchingOptions = _batchingOptionsMonitor.CurrentValue;

            IAsyncEnumerable<IReadOnlyList<KeyValuePair<TKey, TValue>>> messages = channelReader
                .ReadAllAsync(cancellationToken)
                .ChunkAsync(
                    currentBatchingOptions.BatchSize,
                    TimeSpan.FromMilliseconds(currentBatchingOptions.BatchTimeout));

            await _messageHandler.HandleBatchesAsync(messages, cancellationToken);
        }
    }
}