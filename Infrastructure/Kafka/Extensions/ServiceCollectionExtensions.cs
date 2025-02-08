using KafkaInfrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<IOptionsMonitor<KafkaOptions>>()
            .Bind(configuration.GetSection("KafkaOptions"));
        serviceCollection.AddOptions<IOptionsMonitor<ConsumerKafkaOptions>>()
            .Bind(configuration.GetSection("ConsumerKafkaOptions"));
        serviceCollection.AddOptions<IOptionsMonitor<ProducerKafkaOptions>>()
            .Bind(configuration.GetSection("ProducerKafkaOptions"));
        serviceCollection.AddOptions<IOptionsMonitor<BatchingOptions>>()
            .Bind(configuration.GetSection("BatchingOptions"));

        return serviceCollection;
    }
}