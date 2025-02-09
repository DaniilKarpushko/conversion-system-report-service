namespace KafkaInfrastructure.Repositories.Models;

public enum ReportState
{
    Unknown = 0,
    Processed,
    Failed,
    Completed,
}