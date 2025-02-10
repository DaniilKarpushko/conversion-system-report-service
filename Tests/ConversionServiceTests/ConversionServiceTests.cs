using conversionSystemReportService.Extensions;
using conversionSystemReportService.Records;
using conversionSystemReportService.Services;
using conversionSystemReportService.Services.ConversionService;
using FluentAssertions;
using FluentAssertions.Common;
using Google.Protobuf.WellKnownTypes;
using KafkaInfrastructure.Redis;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Proto.Contracts;
using ReportService = conversionSystemReportService.Services.ReportService;

namespace Tests.ConversionServiceTests;

public class ConversionServiceTests : IDisposable
{
    private readonly IConversionService _conversionService;
    private readonly IDictionary<string, ReportResult> _cacheMock;
    private readonly Mock<IReportService> _reportService;
    private readonly ShardedDbContextMoq _contextFactoryMock;

    public ConversionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ShardedDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _contextFactoryMock = new ShardedDbContextMoq(options);

        _cacheMock = new Dictionary<string, ReportResult>();
        _reportService = new Mock<IReportService>();
        _reportService.Setup(x => x.AddReportAsync(It.IsAny<ReportResult>(), It.IsAny<CancellationToken>()))
            .Callback<ReportResult, CancellationToken>((report, CancellationToken) 
                => _cacheMock.Add(report.ToReportDbo().ReportId, report)).Returns(Task.CompletedTask);
        _reportService.Setup(x => x.GetReportStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string reportId, CancellationToken cancellationToken) => _cacheMock[reportId]);

        _conversionService = new ConversionService(_reportService.Object, _contextFactoryMock);
    }

    [Test]
    public async Task ConversionTest_CorrectData_ReturnsCorrectResult()
    {
        using var context = _contextFactoryMock.CreateDbContext(1);

        var requestId = $"1_{DateTime.MinValue.ToUniversalTime()}_{DateTime.MaxValue.ToUniversalTime()}";
        var request = new RequestValue
        {
            ProductId = 1,
            Start = DateTime.MinValue.ToUniversalTime().ToTimestamp(),
            End = DateTime.MaxValue.ToUniversalTime().ToTimestamp(),
            RequestId = requestId,
        };

        await _conversionService.ConvertAsync(request, CancellationToken.None);

        var report = await _reportService.Object.GetReportStatusAsync(requestId, CancellationToken.None);
        var reportDbo = report.ToReportDbo();

        reportDbo.Should().NotBeNull();
        reportDbo.State.Should().Be(ReportState.Completed);
        reportDbo.Ratio.Should().Be(1);
    }

    public void Dispose()
    {
        _contextFactoryMock.Dispose();
    }
}