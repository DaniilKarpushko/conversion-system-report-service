using Confluent.Kafka;
using ConversionSystemReportService.Migratioins;
using conversionSystemReportService.Records;
using conversionSystemReportService.Services;
using ConversionSystemReportService.Services;
using conversionSystemReportService.Services.ConversionService;
using conversionSystemReportService.Services.RequestMessageHandler;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using KafkaInfrastructure.Consumer;
using KafkaInfrastructure.Consumer.Entities;
using KafkaInfrastructure.Extensions;
using KafkaInfrastructure.Redis;
using KafkaInfrastructure.Redis.Extensions;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Options;
using KafkaInfrastructure.Serializers;
using Microsoft.Extensions.Options;
using Proto.Contracts;
using ReportService = conversionSystemReportService.Services.ReportService;

namespace conversionSystemReportService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureService(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddKafkaOptions(configuration);
        serviceCollection.AddRedis(configuration);
        serviceCollection.Configure<ShardingOptions>(configuration.GetSection("ShardingOptions"));

        serviceCollection.AddSingleton<SharedDbContextFactory>();

        serviceCollection.AddScoped<ICacheService<ReportResult>, RedisService<ReportResult>>();
        serviceCollection.AddScoped<IReportService, ReportService>();
        serviceCollection.AddScoped<IConversionService, ConversionService>();
        
        serviceCollection.AddGrpc();
        
        serviceCollection.AddTransient<IDeserializer<RequestKey>, ProtobufKafkaSerializer<RequestKey>>();
        serviceCollection.AddTransient<IDeserializer<RequestValue>, ProtobufKafkaSerializer<RequestValue>>();
        
        serviceCollection.AddScoped<IMessageHandler<RequestKey, RequestValue>, RequestConsumerMessageHandler>();
        serviceCollection
            .AddScoped<IConsumerMessageHandler<RequestKey, RequestValue>,
                ConsumerMessageHandler<RequestKey, RequestValue>>();
        serviceCollection
            .AddScoped<IConsumerMessageReader<RequestKey, RequestValue>,
                ConsumerMessageReader<RequestKey, RequestValue>>();

        serviceCollection.MigrateUp(configuration);
        serviceCollection.AddHostedService<ConsumerMessageReaderService<RequestKey, RequestValue>>();

        return serviceCollection;
    }
    
    private static IServiceCollection MigrateUp(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        foreach (var connectionString in configuration.GetSection("ConnectionStrings").GetChildren())
        {
            var sp = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .AddPostgres()
                    .WithGlobalConnectionString(connectionString.Value)
                    .WithMigrationsIn(typeof(InitialMigrations).Assembly)
                )
                .BuildServiceProvider(false);

            using var scope = sp.CreateScope();
            var runner = sp.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }

        return serviceCollection;
    }
}