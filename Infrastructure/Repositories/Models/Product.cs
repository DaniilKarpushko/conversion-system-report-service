using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("product")]
public class Product
{
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    public string ProductName { get; set; } = string.Empty;
    
    [Column("price")]
    public decimal Price { get; set; }
}