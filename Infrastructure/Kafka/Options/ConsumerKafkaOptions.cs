namespace KafkaInfrastructure.Options;

public class ConsumerKafkaOptions
{
    public string Topic { get; set; } = string.Empty;

    public string Group { get; set; } = string.Empty;

    public string InstanceId { get; set; } = string.Empty;

    public int BufferSize { get; set; } = 1;
}