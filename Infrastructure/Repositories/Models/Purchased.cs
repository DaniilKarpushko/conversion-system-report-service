using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("purchased")]
public class Purchased
{
    [Key]
    [Column("purchase_id")]
    public int PurchaseId { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("product_id")]
    public int ProductId { get; set; }
     
    [Column("purchased_at")]
    public DateTime PurchasedAt { get; set; }
}