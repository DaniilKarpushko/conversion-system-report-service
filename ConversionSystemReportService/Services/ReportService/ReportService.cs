using conversionSystemReportService.Extensions;
using conversionSystemReportService.Records;
using KafkaInfrastructure.Repositories.Entities;
using Microsoft.EntityFrameworkCore;
using KafkaInfrastructure.Redis;


namespace conversionSystemReportService.Services;

public class ReportService : IReportService
{
    private readonly SharedDbContextFactory _shardedDbContextFactory;
    private readonly ICacheService<ReportResult> _cacheService;

    public ReportService(SharedDbContextFactory shardedDbContextFactory, ICacheService<ReportResult> cacheService)
    {
        _shardedDbContextFactory = shardedDbContextFactory;
        _cacheService = cacheService;
    }

    public async Task<ReportResult> GetReportStatusAsync(string reportId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cacheService.GetCacheAsync(reportId, cancellationToken);

            if (result is not null)
                return result;

            await using var dbContext = _shardedDbContextFactory.CreateDbContext(GetProductId(reportId));
            var report = await dbContext.Reports.FirstOrDefaultAsync(
                report => report.ReportId == reportId,
                cancellationToken: cancellationToken);

            return report is null
                ? new ReportResult.Failed(reportId, "Report not found")
                : report.ToReportResult();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var fail = new ReportResult.Failed(reportId, e.Message);
            await _cacheService.SetCacheAsync(reportId, fail, cancellationToken);

            return fail;
        }
    }

    public async Task AddReportAsync(ReportResult report, CancellationToken cancellationToken)
    {
        var reportDbo = report.ToReportDbo();
        try
        {
            await using var dbContext = _shardedDbContextFactory.CreateDbContext(GetProductId(reportDbo.ReportId));
            
            await dbContext.Reports.AddAsync(reportDbo, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await _cacheService.SetCacheAsync(reportDbo.ReportId, report, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            var failed = new ReportResult.Failed(reportDbo.ReportId, e.Message);
            await _cacheService.SetCacheAsync(reportDbo.ReportId, failed, cancellationToken);
        }
    }
    
    private int GetProductId(string reportId)
    {
        return int.Parse(reportId.Split('_')[0]);
    }
}