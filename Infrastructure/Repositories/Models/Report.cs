namespace KafkaInfrastructure.Repositories.Models;

public class Report
{
    public string ReportId { get; set; } = null!;
    
    public double Ratio { get; set; }
    
    public int PurchaseAmount { get; set; }
    
    public ReportState State { get; set; }
}