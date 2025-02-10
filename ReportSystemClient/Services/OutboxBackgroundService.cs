using Client.Contracts;
using Google.Protobuf.WellKnownTypes;
using KafkaInfrastructure.Producer;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ReportSystemClient.Services;

public class OutboxBackgroundService : BackgroundService
{
    private readonly IKafkaProducer<RequestKey, RequestValue> _producer;
    private readonly IServiceScopeFactory _scopeFactory;
    private IOptionsMonitor<OutboxServiceOptions> _options;

    public OutboxBackgroundService( 
        IKafkaProducer<RequestKey, RequestValue> producer,
        IServiceScopeFactory serviceScopeFactory,
        IOptionsMonitor<OutboxServiceOptions> options)
    {
        _producer = producer;
        _scopeFactory = serviceScopeFactory;
        _options = options;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await HandleOutboxAsync(stoppingToken);
        }
    }
    
    private async Task HandleOutboxAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var requests = await dbContext.Requests
                .Take(_options.CurrentValue.BatchSize)
                .ToListAsync(cancellationToken);
            
            foreach (var request in requests)
            {
                await _producer.SendMessageAsync(
                    new RequestKey{ Id = request.RequestId }, 
                    new RequestValue
                    {
                        Start = request.Start.ToUniversalTime().ToTimestamp(),
                        End = request.End.ToUniversalTime().ToTimestamp(),
                        ObjectId = request.ProductId,
                        OrderId = request.RequestId
                    },
                    cancellationToken);
                dbContext.Requests.Remove(request);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await transaction.RollbackAsync(cancellationToken);
            await Task.Delay(_options.CurrentValue.Timeout, cancellationToken);
        }
    }
}