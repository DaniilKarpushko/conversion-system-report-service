using System.Text.Json.Serialization;

namespace conversionSystemReportService.Records;

[JsonDerivedType(typeof(Created), typeDiscriminator: nameof(Created))]
[JsonDerivedType(typeof(Completed), typeDiscriminator: nameof(Completed))]
[JsonDerivedType(typeof(Failed), typeDiscriminator: nameof(Failed))]
public record ReportResult
{
    public sealed record Created(string ReportId) : ReportResult;

    public sealed record Completed(string ReportId, double Conversion, long PurchaseAmount) : ReportResult;
    
    public sealed record Failed(string ReportId, string ErrorMessage) : ReportResult;
};