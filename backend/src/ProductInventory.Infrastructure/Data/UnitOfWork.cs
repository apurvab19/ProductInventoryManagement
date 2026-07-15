using ProductInventory.Application.Common.Interfaces;

namespace ProductInventory.Infrastructure.Data;

public class UnitOfWork(
    ApplicationDbContext context,
    IProductRepository products,
    IItemRepository items) : IUnitOfWork
{
    public IProductRepository Products { get; } = products;

    public IItemRepository Items { get; } = items;

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
