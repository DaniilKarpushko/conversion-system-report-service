namespace KafkaInfrastructure.Repositories.Interfaces;

public interface IProductRepository
{
    public Task<long> GetPurchasedProductAmountAsync(
        int productId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken);
    
    public Task<long> GetViewedProductAmountAsync(
        int productId,
        DateTime start,
        DateTime end,
        CancellationToken cancellationToken);
}