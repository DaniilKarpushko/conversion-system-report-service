using KafkaInfrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKafkaOptions(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.Configure<KafkaOptions>(configuration.GetSection("KafkaOptions"));
        serviceCollection.Configure<ConsumerKafkaOptions>(configuration.GetSection("ConsumerKafkaOptions"));
        serviceCollection.Configure<ProducerKafkaOptions>(configuration.GetSection("ProducerKafkaOptions"));
        serviceCollection.Configure<BatchingOptions>(configuration.GetSection("BatchingOptions"));

        return serviceCollection;
    }
}