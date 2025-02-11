using System.Collections;
using conversionSystemReportService.Extensions;
using conversionSystemReportService.Records;
using conversionSystemReportService.Services;
using conversionSystemReportService.Services.ConversionService;
using FluentAssertions;
using FluentAssertions.Common;
using Google.Protobuf.WellKnownTypes;
using KafkaInfrastructure.Redis;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Interfaces;
using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proto.Contracts;
using ReportService = conversionSystemReportService.Services.ReportService;

namespace Tests.ConversionServiceTests;

public class ConversionServiceTests
{
    private readonly IConversionService _conversionService;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly IReportService _reportService;
    private readonly IDictionary<string, ReportResult> _cache = new Dictionary<string, ReportResult>();

    public ConversionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ShardedDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        Mock<IReportRepository> reportRepositoryMock = new();
        _productRepositoryMock = new Mock<IProductRepository>();

        Mock<ICacheService<ReportResult>> cacheServiceMock = new();
        cacheServiceMock
            .Setup(x => x.GetCacheAsync(It.IsAny<string>(), CancellationToken.None))
            .ReturnsAsync((string id,CancellationToken cts) => _cache[id]);

        cacheServiceMock.Setup(x =>
                x.SetCacheAsync(It.IsAny<string>(), It.IsAny<ReportResult>(), CancellationToken.None))
            .Callback<string, ReportResult, CancellationToken>((id, rr, cts) => _cache[id] = rr)
            .Returns(Task.CompletedTask);

        _reportService = new ReportService(cacheServiceMock.Object, reportRepositoryMock.Object);
        _conversionService = new ConversionService(_reportService, _productRepositoryMock.Object);
    }

    [Test]
    public async Task ConversionTest_CorrectData_ReturnsCorrectResult()
    {
        _productRepositoryMock.Setup(x => x.GetViewedProductAmountAsync(
                It.IsAny<int>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                CancellationToken.None))
            .ReturnsAsync(1);
        _productRepositoryMock.Setup(x => x.GetPurchasedProductAmountAsync(
            It.IsAny<int>(),
            It.IsAny<DateTime>(),
            It.IsAny<DateTime>(),
            CancellationToken.None)).ReturnsAsync(1);
        

        var requestId = $"1_{DateTime.MinValue.ToUniversalTime()}_{DateTime.MaxValue.ToUniversalTime()}";
        var request = new RequestValue
        {
            ProductId = 1,
            Start = DateTime.MinValue.ToUniversalTime().ToTimestamp(),
            End = DateTime.MaxValue.ToUniversalTime().ToTimestamp(),
            RequestId = requestId,
        };

        await _conversionService.ConvertAsync(request, CancellationToken.None);

        var report = await _reportService.GetReportStatusAsync(requestId, CancellationToken.None);
        var reportDbo = report.ToReportDbo();

        reportDbo.Should().NotBeNull();
        reportDbo.State.Should().Be(ReportState.Completed);
        reportDbo.Ratio.Should().Be(1);
    }
}