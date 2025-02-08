using KafkaInfrastructure.Options;
using Microsoft.Extensions.Hosting;
using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer;

public class ConsumerMessageReaderService<TKey, TValue> : BackgroundService
{
    private readonly IConsumerMessageReader<TKey, TValue> _consumerMessageReader;
    private readonly IConsumerMessageHandler<TKey, TValue> _consumerMessageHandler;
    private readonly ConsumerKafkaOptions _consumerKafkaOptions;

    public ConsumerMessageReaderService(
        IConsumerMessageReader<TKey, TValue> consumerMessageReader,
        IConsumerMessageHandler<TKey, TValue> consumerMessageHandler,
        ConsumerKafkaOptions consumerKafkaOptions)
    {
        _consumerMessageReader = consumerMessageReader;
        _consumerMessageHandler = consumerMessageHandler;
        _consumerKafkaOptions = consumerKafkaOptions;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ExecuteTaskAsync(stoppingToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private async Task ExecuteTaskAsync(CancellationToken cancellationToken)
    {
        var channelOptions = new BoundedChannelOptions(_consumerKafkaOptions.BufferSize * 2)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        };
        
        var channel = Channel.CreateBounded<KeyValuePair<TKey, TValue>>(channelOptions);
        
        await Task.WhenAll(
            _consumerMessageHandler.HandleAsync(channel, cancellationToken),
            _consumerMessageReader.ReadAsync(channel.Writer, cancellationToken));
    }
}