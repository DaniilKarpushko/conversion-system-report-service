using System.ComponentModel.DataAnnotations.Schema;

namespace KafkaInfrastructure.Repositories.Models;

[Table("user")]
public class User
{
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("name")]
    public string Name { get; set; } = null!;
    
    [Column("email")]
    public string Email { get; set; } = null!;
}