using KafkaInfrastructure.Repositories.Interfaces;
using Npgsql;

namespace KafkaInfrastructure.Repositories.Entities;

public class ProductRepository : IProductRepository
{
    private readonly INpgsqlConnectionManager _shardedDbManager;

    public ProductRepository(INpgsqlConnectionManager shardedDbManager)
    {
        _shardedDbManager = shardedDbManager;
    }

    public async Task<long> GetPurchasedProductAmountAsync(
        int productId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken)
    {
        var sql = """
                  select count(*) from purchased
                  where product_id = :productId AND purchased_at BETWEEN :start AND :end
                  """;
        var dataSource = _shardedDbManager.GetDataSource(productId);
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("productId", productId),
                new NpgsqlParameter("start", start),
                new NpgsqlParameter("end", end),
            }
        };

        var count = await command.ExecuteScalarAsync(cancellationToken);

        return count is not null ? (long)count : 0;
    }

    public async Task<long> GetViewedProductAmountAsync(
        int productId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken)
    {
        var sql = """
                  select count(*) from viewed
                  where product_id = :productId AND viewed_at BETWEEN :start AND :end
                  """;
        
        var dataSource = _shardedDbManager.GetDataSource(productId);
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("productId", productId),
                new NpgsqlParameter("start", start),
                new NpgsqlParameter("end", end),
            }
        };
        
        var count = await command.ExecuteScalarAsync(cancellationToken);

        return count is not null ? (long)count : 0;
    }
}