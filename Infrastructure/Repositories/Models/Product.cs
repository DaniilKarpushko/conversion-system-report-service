namespace KafkaInfrastructure.Repositories.Models;

public class Product
{
    public string ProductId { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;
    
    public decimal Price { get; set; }
    
    public List<Purchased> PostTags { get; } = [];
    public List<Viewed> ViewedTags { get; } = [];
}