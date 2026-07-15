using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Common.Interfaces;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Infrastructure.Data.Repositories;

public class ItemRepository(ApplicationDbContext context)
    : IItemRepository
{
    public async Task<Item?> GetByProductIdAsync(
        int productId,
        CancellationToken cancellationToken = default)
    {
        return await context.Items
            .AsNoTracking()
            .FirstOrDefaultAsync(
                item => item.ProductId == productId,
                cancellationToken);
    }

    public async Task AddAsync(
        Item item,
        CancellationToken cancellationToken = default)
    {
        await context.Items.AddAsync(item, cancellationToken);
    }

    public void Update(Item item)
    {
        context.Items.Update(item);
    }

    public void Delete(Item item)
    {
        context.Items.Remove(item);
    }
}
