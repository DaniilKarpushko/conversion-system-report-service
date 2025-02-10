using conversionSystemReportService.Extensions;
using conversionSystemReportService.Services;
using Grpc.Core;
using Proto.Contracts;
using ReportService = Proto.Contracts.ReportService;


namespace ConversionSystemReportService.Services;

public class ReportServiceGrpc : ReportService.ReportServiceBase
{
    private readonly IReportService _reportService;

    public ReportServiceGrpc(IReportService reportService)
    {
        _reportService = reportService;
    }

    public override async Task<ReportStatus> GetReport(GetReportRequest request, ServerCallContext context)
    {
        var report = await _reportService.GetReportStatusAsync(request.ReportId, context.CancellationToken);
    
        return report.ToReportStatus();
    }
}