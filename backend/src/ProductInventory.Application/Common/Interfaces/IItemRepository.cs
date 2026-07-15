using ProductInventory.Domain.Entities;

namespace ProductInventory.Application.Common.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Item item,
        CancellationToken cancellationToken = default);

    void Update(Item item);

    void Delete(Item item);
}
