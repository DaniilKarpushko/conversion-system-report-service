namespace KafkaInfrastructure.Redis.Options;

public class RedisOptions
{
    public string Endpoint{ get; set; } = null!;
    
    public int Expiration { get; set; }
}