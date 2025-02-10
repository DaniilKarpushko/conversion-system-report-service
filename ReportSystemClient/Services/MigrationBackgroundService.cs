using FluentMigrator.Runner;

namespace ReportSystemClient.Services;

public class MigrationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public MigrationBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RunMigrationsAsync(stoppingToken);
    }

    private async Task RunMigrationsAsync(CancellationToken cancellationToken)
    {
            await using var scope = _serviceProvider.CreateAsyncScope();
            var migrationRunner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            migrationRunner.MigrateUp();
    }
}