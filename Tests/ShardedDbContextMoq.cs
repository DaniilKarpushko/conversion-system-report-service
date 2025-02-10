using KafkaInfrastructure.Repositories.Entities;
using KafkaInfrastructure.Repositories.Models;
using KafkaInfrastructure.Repositories.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Tests;

public class ShardedDbContextMoq : IShardedDbContextFactory, IDisposable
{
    private readonly DbContextOptions<ShardedDbContext> _options;
    private readonly ShardedDbContext _context;

    public ShardedDbContextMoq(DbContextOptions<ShardedDbContext> options)
    {
        _options = options;
        _context = new ShardedDbContext(_options);
        FullfillContext(_context);
    }

    public ShardedDbContext CreateDbContext(int objectId)
    {
        return _context; // Всегда возвращаем один и тот же контекст
    }

    private void FullfillContext(ShardedDbContext context)
    {
        var product = new Product { ProductId = 1, Price = 100, ProductName = "orange" };
        var user = new User { UserId = 1, Email = "mail", Name = "User" };
        var purchase = new Purchased { ProductId = 1, UserId = 1, PurchasedAt = DateTime.UtcNow, PurchaseId = 1 };
        var view = new Viewed
        {
            ProductId = 1, UserId = 1, ViewedAt = DateTime.UtcNow, ViewId = 1, Product = product, User = user
        };

        context.Products.Add(product);
        context.Users.Add(user);
        context.Viewed.Add(view);
        context.Purchased.Add(purchase);
        
        context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}