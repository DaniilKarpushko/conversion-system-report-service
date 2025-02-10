using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("request")]
public class Request
{
    [Column("product_id")]
    public int ProductId { get; set; }
    
    [Column("request_id")]
    public string RequestId { get; set; } = null!;
    
    [Column("start")]
    public DateTime Start { get; set; }
    
    [Column("end")]
    public DateTime End { get; set; }
}