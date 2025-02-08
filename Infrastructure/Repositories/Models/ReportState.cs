namespace KafkaInfrastructure.Repositories.Models;

public enum ReportState
{
    Unknown = 0,
    Created,
    Processed,
    Failed,
    Done,
}