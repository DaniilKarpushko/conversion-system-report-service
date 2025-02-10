using Confluent.Kafka;
using KafkaInfrastructure.Consumer.Entities;
using KafkaInfrastructure.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer;

public class ConsumerMessageReaderService<TKey, TValue> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IServiceScopeFactory _scopeFactory;
    
    private readonly IDeserializer<TKey> _keyDeserializer;
    private readonly IDeserializer<TValue> _valueDeserializer;
     
    private readonly IOptionsMonitor<ConsumerKafkaOptions> _consumerKafkaOptions;

    public ConsumerMessageReaderService(
        IOptionsMonitor<ConsumerKafkaOptions> consumerKafkaOptions,
        IServiceProvider serviceProvider,
        IServiceScopeFactory scopeFactory)
    {
        _consumerKafkaOptions = consumerKafkaOptions;
        _serviceProvider = serviceProvider;
        _scopeFactory = scopeFactory;

        _keyDeserializer = serviceProvider.GetRequiredService<IDeserializer<TKey>>();
        _valueDeserializer = serviceProvider.GetRequiredService<IDeserializer<TValue>>();
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
        await using var scope = _scopeFactory.CreateAsyncScope();
        var channelOptions = new BoundedChannelOptions(_consumerKafkaOptions.CurrentValue.BufferSize * 2)
        {
            SingleWriter = true,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        };

        var consumerMessageHandler = scope.ServiceProvider.GetRequiredService<IConsumerMessageHandler<TKey, TValue>>();
        var consumerMessageReader = scope.ServiceProvider.GetRequiredService<IConsumerMessageReader<TKey, TValue>>();
        
        var channel = Channel.CreateBounded<KeyValuePair<TKey, TValue>>(channelOptions);
        
        await Task.WhenAll(
            consumerMessageHandler.HandleAsync(channel, cancellationToken),
            consumerMessageReader.ReadAsync(channel.Writer, cancellationToken));
    }
}