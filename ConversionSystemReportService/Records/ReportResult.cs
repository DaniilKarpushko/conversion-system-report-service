namespace conversionSystemReportService.Records;

public record ReportResult
{
    public sealed record InProgress(string ReportId) : ReportResult;
    
    public sealed record Completed(string ReportId, double Conversion, int PurchaseAmount) : ReportResult;
    
    public sealed record Failed(string ReportId, string ErrorMessage) : ReportResult;
};