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
    
    public ShardedReportDbContext CreateDbContext(int objectId)
    {
        var connectionString = GetConnectionString(GetShardNumber(objectId));
        ArgumentNullException.ThrowIfNull(connectionString);

        var options = new DbContextOptionsBuilder<ShardedReportDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        
        return new ShardedReportDbContext(options);

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