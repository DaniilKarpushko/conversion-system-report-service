using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("report")]
public class Report
{
    [Column("report_id")]
    public string ReportId { get; set; } = null!;
    
    [Column("ratio")]
    public double Ratio { get; set; }
    
    [Column("purchase_amount")]
    public long PurchaseAmount { get; set; }
    
    [Column("state")]
    public ReportState State { get; set; }
}