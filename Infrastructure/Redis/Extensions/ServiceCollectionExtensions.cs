using KafkaInfrastructure.Redis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<IOptionsMonitor<RedisOptions>>().Bind(configuration.GetSection("Redis"));

        return serviceCollection;
    }
}