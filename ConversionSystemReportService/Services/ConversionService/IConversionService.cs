using Proto.Contracts;

namespace conversionSystemReportService.Services.ConversionService;

public interface IConversionService
{
    public Task ConvertAsync(RequestValue requestValue, CancellationToken cancellationToken);
}