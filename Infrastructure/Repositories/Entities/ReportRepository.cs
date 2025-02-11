using System.Data;
using KafkaInfrastructure.Repositories.Interfaces;
using KafkaInfrastructure.Repositories.Models;
using Npgsql;

namespace KafkaInfrastructure.Repositories.Entities;

public class ReportRepository : IReportRepository
{
    private INpgsqlConnectionManager _shardedDbManager;

    public ReportRepository(INpgsqlConnectionManager shardedDbManager)
    {
        _shardedDbManager = shardedDbManager;
    }

    public async Task CreateReportAsync(Report report, CancellationToken cancellationToken)
    {
        var dataSource = _shardedDbManager.GetDataSource(GetProductId(report.ReportId));
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command =
            new NpgsqlCommand(
                """
                INSERT INTO report (report_id, state, purchase_amount, ratio)
                VALUES (:reportId, :state, :purchaseAmount, :ratio);
                """,
                connection)
            {
                Parameters =
                {
                    new NpgsqlParameter("reportId", report.ReportId),
                    new NpgsqlParameter("state", (int)report.State),
                    new NpgsqlParameter("purchaseAmount", report.PurchaseAmount),
                    new NpgsqlParameter("ratio", report.Ratio),
                }
            };
        
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<Report> GetReportAsync(string reportId, CancellationToken cancellationToken)
    {
        var sql = $"SELECT * FROM report WHERE report_id = (:reportId)";
        
        var dataSource = _shardedDbManager.GetDataSource(GetProductId(reportId));
        await using var connection = await dataSource.OpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new NpgsqlParameter("reportId", reportId),
            },
        };
            
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

        if (await reader.ReadAsync(cancellationToken))
        {
            return new Report
            {
                ReportId = reader.GetString("report_id"),
                State = (ReportState)reader.GetInt32("state"),
                PurchaseAmount = reader.GetInt32("purchase_amount"),
                Ratio = reader.GetDouble("ratio"),
            };
        }

        return new Report{State = ReportState.Unknown};
    }
    
    private int GetProductId(string reportId)
    {
        return int.Parse(reportId.Split('_')[0]);
    }
}