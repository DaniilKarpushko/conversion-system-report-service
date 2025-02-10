using conversionSystemReportService.Records;

namespace conversionSystemReportService.Services;

public interface IReportService
{
    public Task<ReportResult> GetReportStatusAsync(string reportId, CancellationToken cancellationToken); 
    
    public Task AddReportAsync(ReportResult report, CancellationToken cancellationToken);
}