using Confluent.Kafka;
using KafkaInfrastructure.Options;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Producer;

public class KafkaProducer<TKey, TValue> : IKafkaProducer<TKey, TValue>, IDisposable
{
    private readonly IProducer<TKey, TValue> _producer;
    private readonly IOptionsMonitor<ProducerKafkaOptions> _options;

    public KafkaProducer(
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer,
        IOptionsMonitor<KafkaOptions> kafkaOptions,
        IOptionsMonitor<ProducerKafkaOptions> producerKafkaOptions)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = kafkaOptions.CurrentValue.Host,
        };

        _options = producerKafkaOptions;
        
        _producer = new ProducerBuilder<TKey, TValue>(config)
            .SetValueSerializer(valueSerializer)
            .SetKeySerializer(keySerializer)
            .Build();
        
    }

    public async Task SendMessageAsync(TKey key, TValue value, CancellationToken cancellationToken)
    {
        try
        {
            var message = new Message<TKey, TValue> { Key = key, Value = value };
            await _producer.ProduceAsync(_options.CurrentValue.Topic, message, cancellationToken);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void Dispose() => _producer.Dispose();
}