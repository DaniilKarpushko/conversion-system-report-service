using ReportSystemClient.Extensions;
using Client.Contracts;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Models;
using ReportSystemClient.Records;

namespace ReportSystemClient.Services.ReportService;

public class ReportClientService : IReportClientService
{
    private readonly Client.Contracts.ReportService.ReportServiceClient _reportServiceClient;
    private readonly OutboxRepository _outboxRepository;

    public ReportClientService(OutboxRepository outboxRepository, Client.Contracts.ReportService.ReportServiceClient reportServiceClient)
    {
        _outboxRepository = outboxRepository;
        _reportServiceClient = reportServiceClient;
    }

    public async Task<RequestResult> CreateReportAsync(
        int productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = GenerateRequestKey(productId, startDate, endDate);
            var request = new Request
            {
                ProductId = productId,
                Start = startDate,
                End = endDate,
                RequestId = id,
            };
            
            await _outboxRepository.Requests.AddAsync(request, cancellationToken);
            await _outboxRepository.SaveChangesAsync(cancellationToken);

            return new RequestResult.Success(id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
            return new RequestResult.Failure(e.Message);
        }
    }

    public async Task<ReportResult> GetReportAsync(string reportId, CancellationToken cancellationToken)
    {
        var request = new GetReportRequest{ ReportId = reportId };
        var result = await _reportServiceClient.GetReportAsync(request, cancellationToken: cancellationToken);

        return result is not null 
            ? result.ToRecord() 
            : new ReportResult.Failed(reportId, "Report not found");
    }

    private string GenerateRequestKey(int productId, DateTime start, DateTime end)
    {
        return $"{productId}_{start}_{end}";
    }
}