using System.Text.Json.Serialization;

namespace ReportSystemClient.Records;

[JsonDerivedType(typeof(Completed), typeDiscriminator: nameof(Completed))]
[JsonDerivedType(typeof(Failed), typeDiscriminator: nameof(Failed))]
public record ReportResult
{
    public sealed record Completed(string ReportId, double Conversion, int PurchaseAmount) : ReportResult;

    public sealed record Failed(string ReportFailedReportId, string ReportFailedReason) : ReportResult;
};