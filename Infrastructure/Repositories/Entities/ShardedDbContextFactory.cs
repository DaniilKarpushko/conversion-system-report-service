using KafkaInfrastructure.Repositories.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace KafkaInfrastructure.Repositories.Entities;

public interface IShardedDbContextFactory
{
    public ShardedDbContext CreateDbContext(int objectId);
}

public class ShardedDbContextFactory: IShardedDbContextFactory
{
    private readonly IConfiguration _configuration;
    private readonly IOptions<ShardingOptions> _shardingOptions;
    
    public ShardedDbContextFactory(IConfiguration configuration, IOptions<ShardingOptions> shardingOptions)
    {
        _configuration = configuration;
        _shardingOptions = shardingOptions;
    }
    
    public ShardedDbContext CreateDbContext(int objectId)
    {
        var connectionString = GetConnectionString(GetShardNumber(objectId));
        ArgumentNullException.ThrowIfNull(connectionString);

        var options = new DbContextOptionsBuilder<ShardedDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        return new ShardedDbContext(options);

    }

    private int GetShardNumber(int productId)
    {
        return productId % _shardingOptions.Value.ShardCount;
    }
    
    private string? GetConnectionString(int shardId)
    {
        return _configuration.GetConnectionString($"Products_{shardId}");
    }
}