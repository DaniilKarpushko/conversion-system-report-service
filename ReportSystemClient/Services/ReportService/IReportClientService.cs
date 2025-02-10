using Client.Contracts;
using ReportSystemClient.Records;

namespace ReportSystemClient.Services.ReportService;

public interface IReportClientService
{
    public Task<RequestResult> CreateReportAsync(
        int productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken);
    
    public Task<ReportResult> GetReportAsync(string reportId, CancellationToken cancellationToken);
}