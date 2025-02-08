using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInfrastructure.Repositories.Entities;

public class ShardedDbContext : DbContext
{
    public ShardedDbContext(DbContextOptions<ShardedDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Purchased> Purchased { get; set; }
    
    public DbSet<Viewed> Viewed { get; set; }
}