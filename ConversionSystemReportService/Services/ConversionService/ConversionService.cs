using conversionSystemReportService.Records;
using KafkaInfrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using Proto.Contracts;


namespace conversionSystemReportService.Services.ConversionService;

public class ConversionService : IConversionService
{
    private readonly IReportService _reportService;
    private readonly SharedDbContextFactory _shardedDbContextFactory;

    public ConversionService(IReportService reportService, SharedDbContextFactory shardedDbContextFactory)
    {
        _reportService = reportService;
        _shardedDbContextFactory = shardedDbContextFactory;
    }

    public async Task ConvertAsync(RequestValue requestValue, CancellationToken cancellationToken)
    {
        
        try
        {
            Console.WriteLine("startedConversion");
           await using var dbContext = _shardedDbContextFactory.CreateDbContext(requestValue.ProductId);
           
           var purchaseCount = await dbContext.Purchased
               .Where(o => o.ProductId == requestValue.ProductId
                           && o.PurchasedAt >= requestValue.Start.ToDateTime() 
                           && o.PurchasedAt <= requestValue.End.ToDateTime())
               .CountAsync(cancellationToken);
           
           var viewCount = await dbContext.Viewed
               .Where(v => v.ProductId == requestValue.ProductId
                           && v.ViewedAt >= requestValue.Start.ToDateTime() 
                           && v.ViewedAt <= requestValue.End.ToDateTime())
               .CountAsync(cancellationToken);

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