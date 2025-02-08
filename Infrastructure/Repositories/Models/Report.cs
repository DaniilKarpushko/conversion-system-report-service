namespace KafkaInfrastructure.Repositories.Models;

public class Report
{
    public string ReportId { get; set; } = null!;
    
    public double Ratio { get; set; }
    
    public uint PurchaseAmount { get; set; }
    
    public ReportState State { get; set; }
}