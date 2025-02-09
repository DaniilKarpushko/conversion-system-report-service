using conversionSystemReportService.Records;
using Request.Kafka.Contracts;

namespace conversionSystemReportService.Services.ReportService;

public interface IReportService
{
    public Task<ReportResult> GetReportStatusAsync(string reportId, CancellationToken cancellationToken); 
    
    public Task CreateReportAsync(ReportResult reportObj, CancellationToken cancellationToken);

    public Task UpdateReportAsync(ReportResult reportResult, CancellationToken cancellationToken);
}