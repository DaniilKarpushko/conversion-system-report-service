using Confluent.Kafka;
using KafkaInfrastructure.Options;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace KafkaInfrastructure.Consumer.Entities;

public class ConsumerMessageReader<TKey, TValue> : IConsumerMessageReader<TKey, TValue>
{
    private readonly KafkaOptions _kafkaOptions;
    private readonly IOptionsMonitor<ConsumerKafkaOptions> _kafkaConsumerOptions;
    private readonly IDeserializer<TValue> _valueDeserializer;
    private readonly IDeserializer<TKey> _keyDeserializer;

    public ConsumerMessageReader(
        IOptions<KafkaOptions> kafkaOptions,
        IOptionsMonitor<ConsumerKafkaOptions> kafkaConsumerOptions,
        IDeserializer<TValue> valueDeserializer,
        IDeserializer<TKey> keyDeserializer)
    {
        _kafkaOptions = kafkaOptions.Value;
        _kafkaConsumerOptions = kafkaConsumerOptions;
        _valueDeserializer = valueDeserializer;
        _keyDeserializer = keyDeserializer;
    }


    public async Task ReadAsync(ChannelWriter<KeyValuePair<TKey, TValue>> channelWriter, CancellationToken cancellationToken)
    {
        using var consumer = ConfigureConsumer();
        
        consumer.Subscribe(_kafkaConsumerOptions.CurrentValue.Topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<TKey, TValue> consumeResult = consumer.Consume(cancellationToken);
                
                var message = new KeyValuePair<TKey, TValue>(consumeResult.Message.Key, consumeResult.Message.Value);
                await channelWriter.WriteAsync(message, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private IConsumer<TKey, TValue> ConfigureConsumer()
    {
        var consumerConfiguration = new ConsumerConfig
        {
            BootstrapServers = _kafkaOptions.Host,
            GroupId = _kafkaConsumerOptions.CurrentValue.Group,
            GroupInstanceId = _kafkaConsumerOptions.CurrentValue.InstanceId,
            EnableAutoCommit = false,
        };

        return new ConsumerBuilder<TKey, TValue>(consumerConfiguration)
            .SetKeyDeserializer(_keyDeserializer)
            .SetValueDeserializer(_valueDeserializer)
            .Build();
    }
}