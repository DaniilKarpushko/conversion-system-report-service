using conversionSystemReportService.Extensions;
using conversionSystemReportService.Records;
using KafkaInfrastructure.Redis.RedisService;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Request.Kafka.Contracts;
using System.Text;

namespace conversionSystemReportService.Services.ReportService;

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

    public async Task CreateReportAsync(ReportResult reportObj, CancellationToken cancellationToken)
    {
        try
        {
            var report = reportObj.ToReport();
            await using var dbContext = _shardedDbContextFactory.CreateDbContext(GetProductId(report.ReportId));

            await dbContext.Reports.AddAsync(report, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            await _cacheService.SetCacheAsync(report.ReportId, reportObj, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task UpdateReportAsync(ReportResult reportResult, CancellationToken cancellationToken)
    {
        try
        {
            var reportConverted = reportResult.ToReport();
            DbSet<Report> reports = _shardedDbContextFactory.CreateDbContext(reportConverted.ReportId).Reports;
            
            await reports.Where(report => report.ReportId == reportConverted.ReportId)
                .ExecuteUpdateAsync(
                    x => x.SetProperty(r => r.State, reportConverted.State),
                    cancellationToken);
            
            await _cacheService.SetCacheAsync(reportConverted.ReportId, reportResult, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private string GetProductId(string reportId)
    {
        return reportId.Split('_')[0];
    }
}