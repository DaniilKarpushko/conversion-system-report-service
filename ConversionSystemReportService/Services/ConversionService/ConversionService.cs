using conversionSystemReportService.Records;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Proto.Contracts;


namespace conversionSystemReportService.Services.ConversionService;

public class ConversionService : IConversionService
{
    private readonly IReportService _reportService;
    private readonly IProductRepository _shardedDbContextFactory;

    public ConversionService(IReportService reportService, IProductRepository shardedDbContextFactory)
    {
        _reportService = reportService;
        _shardedDbContextFactory = shardedDbContextFactory;
    }

    public async Task ConvertAsync(RequestValue requestValue, CancellationToken cancellationToken)
    {
        try
        {
            var purchaseCount = await _shardedDbContextFactory.GetPurchasedProductAmountAsync(
                requestValue.ProductId,
                requestValue.Start.ToDateTime(),
                requestValue.End.ToDateTime(),
                cancellationToken);
            
            var viewCount = await _shardedDbContextFactory.GetViewedProductAmountAsync(
                requestValue.ProductId,
                requestValue.Start.ToDateTime(),
                requestValue.End.ToDateTime(),
                cancellationToken);
            
           var ratio = (double)purchaseCount / viewCount;
           var report = new ReportResult.Completed(requestValue.RequestId, ratio, purchaseCount);

           await _reportService.AddReportAsync(report, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}