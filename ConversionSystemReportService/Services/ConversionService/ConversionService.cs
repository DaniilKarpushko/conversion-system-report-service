using conversionSystemReportService.Records;
using conversionSystemReportService.Services.ReportService;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Request.Kafka.Contracts;

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
           await using var dbContext = _shardedDbContextFactory.CreateDbContext(requestValue.ObjectId);
           await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
           
           var purchaseCount = await dbContext.Purchased
               .Where(o => o.ProductId == requestValue.ObjectId 
                           && o.PurchasedAt >= requestValue.Start.ToDateTime() 
                           && o.PurchasedAt <= requestValue.End.ToDateTime())
               .CountAsync(cancellationToken);
           
           var viewCount = await dbContext.Viewed
               .Where(v => v.ProductId == requestValue.ObjectId 
                           && v.ViewedAt >= requestValue.Start.ToDateTime() 
                           && v.ViewedAt <= requestValue.End.ToDateTime())
               .CountAsync(cancellationToken);

           var ratio = (double)purchaseCount / viewCount;
           var report = new ReportResult.Completed(CreateReportId(requestValue), ratio, purchaseCount);

           await _reportService.CreateReportAsync(report, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    private string CreateReportId(RequestValue requestValue)
    {
        return $"{requestValue.ObjectId}_{requestValue.OrderId}_{requestValue.Start}_{requestValue.End}";
    }
}