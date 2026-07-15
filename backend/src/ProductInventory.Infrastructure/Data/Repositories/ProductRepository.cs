using Microsoft.EntityFrameworkCore;
using ProductInventory.Application.Common.Interfaces;
using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Products;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Infrastructure.Data.Repositories;

public class ProductRepository(ApplicationDbContext context)
    : IProductRepository
{
    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await context.Products
            .Include(product => product.Item)
            .AsNoTracking()
            .FirstOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<PagedResult<Product>> GetPagedAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var pageNumber = Math.Max(parameters.PageNumber, 1);
        var pageSize = Math.Clamp(parameters.PageSize, 1, 100);

        IQueryable<Product> query = context.Products
            .Include(product => product.Item)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();

            query = query.Where(product =>
                EF.Functions.Like(product.ProductName, $"%{search}%"));
        }

        var descending = parameters.SortDirection
            .Equals("desc", StringComparison.OrdinalIgnoreCase);

        query = parameters.SortBy.ToLowerInvariant() switch
        {
            "createdon" => descending
                ? query.OrderByDescending(product => product.CreatedOn)
                : query.OrderBy(product => product.CreatedOn),

            "quantity" => descending
                ? query.OrderByDescending(product =>
                    product.Item != null ? product.Item.Quantity : 0)
                : query.OrderBy(product =>
                    product.Item != null ? product.Item.Quantity : 0),

            _ => descending
                ? query.OrderByDescending(product => product.ProductName)
                : query.OrderBy(product => product.ProductName)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        context.Products.Remove(product);
    }
}
