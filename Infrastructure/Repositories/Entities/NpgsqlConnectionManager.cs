using KafkaInfrastructure.Repositories.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace KafkaInfrastructure.Repositories.Entities;

public interface INpgsqlConnectionManager
{
    public NpgsqlDataSource GetDataSource(int productId);
}
public class NpgsqlConnectionManager : INpgsqlConnectionManager
{
    private readonly IConfiguration _configuration;
    private readonly IOptionsMonitor<ShardingOptions> _shardingOptions;
    private IDictionary<int, NpgsqlDataSource> _dataSources = new Dictionary<int, NpgsqlDataSource>();

    public NpgsqlConnectionManager(IConfiguration configuration, IOptionsMonitor<ShardingOptions> shardingOptions)
    {
        _configuration = configuration;
        _shardingOptions = shardingOptions;
    }

    public NpgsqlDataSource GetDataSource(int productId)
    {
        if (_dataSources.TryGetValue(productId % _shardingOptions.CurrentValue.ShardCount, out var dataSource))
            return dataSource;
        
        var connectionString = _configuration.GetConnectionString($"Products_{productId % _shardingOptions.CurrentValue.ShardCount}");
        dataSource = new NpgsqlDataSourceBuilder(connectionString).Build();
        _dataSources[productId % _shardingOptions.CurrentValue.ShardCount] = dataSource;
        
        return dataSource;
    }
}