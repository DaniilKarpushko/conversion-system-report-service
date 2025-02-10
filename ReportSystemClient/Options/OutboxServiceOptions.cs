namespace KafkaInfrastructure.Repositories.Options;

public class OutboxServiceOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    
    public int BatchSize { get; set; }
    
    public int Timeout { get; set; }
}