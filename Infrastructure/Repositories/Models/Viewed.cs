using Confluent.Kafka;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("viewed")]
public class Viewed
{
    [Key]
    [Column("view_id")]
    public int ViewId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("product_id")]
    public int ProductId { get; set; }
    
    [Column("product")]
    public Product Product { get; set; } = null!;
    
    [Column("user")]
    public User User { get; set; } = null!;
    
    [Column("viewed_at")]
    public DateTime ViewedAt { get; set; }
}