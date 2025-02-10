using KafkaInfrastructure.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInfrastructure.Repositories.Entities;

public class OutboxRepository : DbContext
{
    public OutboxRepository(DbContextOptions<OutboxRepository> options) : base(options) { }

    public DbSet<Request> Requests { get; set; }
}