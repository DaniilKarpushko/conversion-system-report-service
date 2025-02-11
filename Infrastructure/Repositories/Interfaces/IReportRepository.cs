using KafkaInfrastructure.Repositories.Models;

namespace KafkaInfrastructure.Repositories.Interfaces;

public interface IReportRepository
{
    public Task CreateReportAsync(Report report, CancellationToken cancellationToken);
    
    public Task<Report> GetReportAsync(string reportId, CancellationToken cancellationToken);
}