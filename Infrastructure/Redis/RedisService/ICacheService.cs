namespace KafkaInfrastructure.Redis;

public interface ICacheService<T>
{
    public Task SetCacheAsync(string key, T value, CancellationToken cancellationToken);

    public Task<T?> GetCacheAsync(string key, CancellationToken cancellationToken);
}