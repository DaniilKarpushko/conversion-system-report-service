using ApiGateway.Migrations;
using ApiGateway.Options;
using Client.Contracts;
using Confluent.Kafka;
using FluentMigrator.Runner;
using KafkaInfrastructure.Extensions;
using KafkaInfrastructure.Producer;
using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Options;
using KafkaInfrastructure.Serializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReportSystemClient.Services;
using ReportSystemClient.Services.ReportService;

namespace ReportSystemClient.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReportSystemClient(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.Configure<GrpcOptions>(
            configuration.GetSection("GrpcOptions"));
        
        serviceCollection.AddKafkaOptions(configuration);
        
        serviceCollection.Configure<OutboxServiceOptions>(configuration.GetSection("OutboxServiceOptions"));
        serviceCollection.AddMigrations();
        serviceCollection.AddDbContext<OutboxRepository>((sp, builder) =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<OutboxServiceOptions>>().CurrentValue;
            builder.UseNpgsql(options.ConnectionString);
        });
        
        serviceCollection.AddTransient<ISerializer<RequestKey>, ProtobufKafkaSerializer<RequestKey>>();
        serviceCollection.AddTransient<ISerializer<RequestValue>, ProtobufKafkaSerializer<RequestValue>>();
        
        serviceCollection.AddSingleton<IKafkaProducer<RequestKey, RequestValue>, KafkaProducer<RequestKey, RequestValue>>();

        serviceCollection.AddHostedService<MigrationBackgroundService>();
        serviceCollection.AddHostedService<OutboxBackgroundService>();
        
        serviceCollection.AddGrpcClient<ReportService.ReportServiceClient>((sp, o) =>
        {
            var options = sp.GetRequiredService<IOptions<GrpcOptions>>();
            o.Address = new Uri(options.Value.Address);
        });
        
        serviceCollection.AddScoped<IReportClientService, ReportClientService>();

        serviceCollection.AddControllers();
        return serviceCollection;
    }

    private static IServiceCollection AddMigrations(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    var options = provider.GetRequiredService<IOptionsMonitor<OutboxServiceOptions>>();
                    return options.CurrentValue.ConnectionString;
                })
                .WithMigrationsIn(typeof(InitialMigrations).Assembly));

        return serviceCollection;
    }
}