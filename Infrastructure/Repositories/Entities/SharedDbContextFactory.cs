using KafkaInfrastructure.Repositories.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Repositories.Entities;

public class SharedDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<ShardingOptions> _shardingOptions;
    
    public SharedDbContextFactory(IConfiguration configuration, IOptions<ShardingOptions> shardingOptions)
    {
        _configuration = configuration;
        _shardingOptions = shardingOptions;
    }
    
    public ShardedDbContext CreateDbContext(string objectId)
    {
        var connectionString = GetConnectionString(objectId);
        ArgumentNullException.ThrowIfNull(connectionString);

        var options = new DbContextOptionsBuilder<ShardedDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        return new ShardedDbContext(options);

    }

    public int GetShardNumber(string productId)
    {
        return productId.GetHashCode(StringComparison.Ordinal) % _shardingOptions.Value.ShardCount;
    }
    
    private string? GetConnectionString(string shardId)
    {
        return _configuration.GetConnectionString($"Products_{shardId}");
    }
}