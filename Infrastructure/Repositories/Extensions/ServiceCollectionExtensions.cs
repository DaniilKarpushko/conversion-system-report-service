using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaInfrastructure.Repositories.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddOptions<ShardingOptions>().Bind(configuration.GetSection("ShardingOptions"));
        serviceCollection.AddSingleton<SharedDbContextFactory>();
        
        return serviceCollection;
    }
}