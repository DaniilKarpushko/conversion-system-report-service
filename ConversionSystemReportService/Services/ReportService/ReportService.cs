using conversionSystemReportService.Extensions;
using conversionSystemReportService.Records;
using KafkaInfrastructure.Redis;
using KafkaInfrastructure.Repositories.Interfaces;
using KafkaInfrastructure.Repositories.Models;

namespace conversionSystemReportService.Services;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly ICacheService<ReportResult> _cacheService;

    public ReportService(ICacheService<ReportResult> cacheService, IReportRepository reportRepository)
    {
        _cacheService = cacheService;
        _reportRepository = reportRepository;
    }

    public async Task<ReportResult> GetReportStatusAsync(string reportId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _cacheService.GetCacheAsync(reportId, cancellationToken);
            if (result is not null)
                return result;

            var report = await _reportRepository.GetReportAsync(reportId, cancellationToken);
            return report.State == ReportState.Unknown 
                ? new ReportResult.Failed(reportId, "Report do not found in repository") 
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
            await _reportRepository.CreateReportAsync(reportDbo, cancellationToken);
            await _cacheService.SetCacheAsync(reportDbo.ReportId, report, cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            var failed = new ReportResult.Failed(reportDbo.ReportId, e.Message);
            await _cacheService.SetCacheAsync(reportDbo.ReportId, failed, cancellationToken);

            throw;
        }
    }
}