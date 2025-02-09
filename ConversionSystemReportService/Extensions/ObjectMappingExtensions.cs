using conversionSystemReportService.Records;
using KafkaInfrastructure.Repositories.Models;

namespace conversionSystemReportService.Extensions;

public static class ObjectMappingExtensions
{
    public static ReportResult ToReportResult(this Report report)
    {
        return report.State switch
        {
            ReportState.Completed => new ReportResult.Completed(report.ReportId, report.Ratio, report.PurchaseAmount),
            ReportState.Failed => new ReportResult.Failed(report.ReportId, "UnknownReason"),
            _ => new ReportResult.Failed(report.ReportId, "Cannot convert type of report"),
        };
    }
    
    public static Report ToReport(this ReportResult reportResult)
    {
        return reportResult switch
        {
            ReportResult.Completed reportCompleted => new Report
            {
                ReportId = reportCompleted.ReportId,
                State = ReportState.Completed,
                Ratio = reportCompleted.Conversion,
                PurchaseAmount = reportCompleted.PurchaseAmount,
            },
            ReportResult.Failed reportFailed => new Report
            {
                ReportId = reportFailed.ReportId,
                State = ReportState.Failed,
                Ratio = 0,
                PurchaseAmount = 0,
            },
            _ => throw new ArgumentException("Cannot convert type of report result", nameof(reportResult)),
        };
    }
}