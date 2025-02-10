using Client.Contracts;
using ReportSystemClient.Records;

namespace ReportSystemClient.Extensions;

public static class ObjectMappingExtensions
{
    public static ReportResult ToRecord(this ReportStatus report)
    {
        return report.ReportStatusCase switch
        {
            ReportStatus.ReportStatusOneofCase.ReportCompleted => new ReportResult.Completed(
                report.ReportCompleted.ReportId,
                report.ReportCompleted.Ratio,
                1),
            ReportStatus.ReportStatusOneofCase.ReportFailed => new ReportResult.Failed(
                report.ReportFailed.ReportId,
                report.ReportFailed.Reason),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}