using conversionSystemReportService.Records;

namespace conversionSystemReportService.Services.ReportService;

public interface IReportService
{
    public Task<ReportResult> GetReportStatus(string reportId); 
    
    public Task CreateReport(string reportId, string productId, string orderId);

    public Task UpdateReport(ReportResult reportResult);
}