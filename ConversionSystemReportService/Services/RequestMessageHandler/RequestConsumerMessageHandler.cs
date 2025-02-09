using conversionSystemReportService.Services.ConversionService;
using conversionSystemReportService.Services.ReportService;
using KafkaInfrastructure.Consumer;
using Request.Kafka.Contracts;

namespace conversionSystemReportService.Services.RequestMessageHandler;

public class RequestConsumerMessageHandler : IMessageHandler<RequestKey, RequestValue>
{
    private readonly IConversionService _conversionService;
    
    public async Task HandleBatchesAsync(
        IAsyncEnumerable<IReadOnlyList<KeyValuePair<RequestKey, RequestValue>>> messageBatches,
        CancellationToken cancellationToken)
    {
        await foreach (var messageBatch in messageBatches.WithCancellation(cancellationToken))
        {
           await ConvertBatchAsync(messageBatch, cancellationToken);
        }
    }

    private async Task ConvertBatchAsync(
        IReadOnlyList<KeyValuePair<RequestKey, RequestValue>> messageBatch,
        CancellationToken cancellationToken)
    {
        foreach (var message in messageBatch)
        {
            await _conversionService.ConvertAsync(message.Value, cancellationToken);
        }
    }
}