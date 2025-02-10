using ConversionSystemReportService.Migratioins;
using conversionSystemReportService.Records;
using FluentMigrator.Runner;
using KafkaInfrastructure.Repositories.Models;
using Microsoft.Extensions.Options;
using Proto.Contracts;

namespace conversionSystemReportService.Extensions;

public static class ObjectMappingExtensions
{
    public static ReportResult ToReportResult(this KafkaInfrastructure.Repositories.Models.Report report)
    {
        return report.State switch
        {
            ReportState.Completed => new ReportResult.Completed(report.ReportId, report.Ratio, report.PurchaseAmount),
            ReportState.Failed => new ReportResult.Failed(report.ReportId, "UnknownReason"),
            _ => new ReportResult.Failed(report.ReportId, "Cannot convert type of report"),
        };
    }
    
    public static KafkaInfrastructure.Repositories.Models.Report ToReportDbo(this ReportResult reportResult)
    {
        return reportResult switch
        {
            ReportResult.Completed reportCompleted => new KafkaInfrastructure.Repositories.Models.Report
            {
                ReportId = reportCompleted.ReportId,
                State = ReportState.Completed,
                Ratio = reportCompleted.Conversion,
                PurchaseAmount = reportCompleted.PurchaseAmount,
            },
            ReportResult.Failed reportFailed => new KafkaInfrastructure.Repositories.Models.Report
            {
                ReportId = reportFailed.ReportId,
                State = ReportState.Failed,
                Ratio = 0,
                PurchaseAmount = 0,
            },
            _ => throw new ArgumentException("Cannot convert type of report result", nameof(reportResult)),
        };
    }

    public static ReportStatus ToReportStatus(this ReportResult reportResult)
    {
        return reportResult switch
        {
            ReportResult.Completed reportCompleted => new ReportStatus
            {
                ReportCompleted = new ReportCompleted
                {
                    ReportId = reportCompleted.ReportId,
                    Ratio = reportCompleted.Conversion,
                    Count = reportCompleted.PurchaseAmount,
                }
            },
            ReportResult.Failed reportFailed => new ReportStatus
            {
                ReportFailed = new ReportFailed
                {
                    ReportId = reportFailed.ReportId,
                    Reason = reportFailed.ErrorMessage,
                }
            },
            _ => throw new ArgumentException("Cannot convert type of report result", nameof(reportResult)),
        };
    }
}