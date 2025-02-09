﻿using KafkaInfrastructure.Redis.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace KafkaInfrastructure.Redis.RedisService;

public class RedisService<T> : ICacheService<T>
{
    private readonly ConnectionMultiplexer _connectionMultiplexer;
    private IOptionsMonitor<RedisOptions> _redisOptions;

    public RedisService(IOptionsMonitor<RedisOptions> redisOptions)
    {
        var config = new ConfigurationOptions
        {
            EndPoints = { redisOptions.CurrentValue.Endpoint },
        };
        
        _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
        _redisOptions = redisOptions;
    }

    public async Task SetCacheAsync(string key, T value, CancellationToken cancellationToken)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var json = JsonSerializer.Serialize(value);
        
            await database.StringSetAsync(key, json, TimeSpan.FromSeconds(_redisOptions.CurrentValue.Expiration));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<T?> GetCacheAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var database = _connectionMultiplexer.GetDatabase();
            var json = await database.StringGetAsync(key);
            
            return json is { HasValue: true, IsNull: false } ? JsonSerializer.Deserialize<T>(json) : default;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return default;
        }
    }
}