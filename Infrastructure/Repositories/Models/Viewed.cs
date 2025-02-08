using Confluent.Kafka;

namespace KafkaInfrastructure.Repositories.Models;

public class Viewed
{
    public string UserId { get; set; } = null!;
    
    public string ProductId { get; set; } = null!;
    
    public Product Product { get; set; } = null!;
    
    public User User { get; set; } = null!;
    
     public DateTime ViewedAt { get; set; }
}