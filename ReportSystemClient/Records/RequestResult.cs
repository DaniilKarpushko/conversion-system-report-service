namespace ReportSystemClient.Records;

public record RequestResult
{
    public sealed record Success(string ReportId) : RequestResult;
    
    public sealed record Failure(string Message) : RequestResult;
};