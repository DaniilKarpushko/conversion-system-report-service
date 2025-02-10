using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInfrastructure.Repositories.Entities;

public class ShardedReportDbContext : DbContext
{
    public ShardedReportDbContext(DbContextOptions<ShardedReportDbContext> options) : base(options) { }
    
    public DbSet<User> Users { get; set; }
    
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Purchased> Purchased { get; set; }
    
    public DbSet<Viewed> Viewed { get; set; }
    
    public DbSet<Report> Reports { get; set; }
}