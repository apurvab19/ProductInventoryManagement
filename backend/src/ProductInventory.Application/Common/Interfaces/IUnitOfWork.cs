namespace ProductInventory.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }

    IItemRepository Items { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
