using KafkaInfrastructure.Consumer;
using Request.Kafka.Contracts;

namespace conversionSystemReportService.Services.RequestMessageHandler;

public class RequestConsumerMessageHandler : IMessageHandler<RequestKey, RequestValue>
{
    public Task HandleMessagesAsync(IAsyncEnumerable<IReadOnlyList<KeyValuePair<RequestKey, RequestValue>>> messageBatches, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}