namespace KafkaInfrastructure.Options;

public class BatchingOptions
{
    public int BatchSize { get; set; } = 100;
    public int BatchTimeout { get; set; } = 1000;
}