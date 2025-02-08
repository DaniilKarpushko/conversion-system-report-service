namespace KafkaInfrastructure.Repositories.Models;

public class Purchased
{
    
    public string UserId { get; set; } = null!;
    
    public string ProductId { get; set; } = null!;
     
    public DateTime PurchasedAt { get; set; }
}