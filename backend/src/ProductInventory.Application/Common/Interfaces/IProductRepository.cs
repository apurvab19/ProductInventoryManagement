using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Products;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Application.Common.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetPagedAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Product product,
        CancellationToken cancellationToken = default);

    void Update(Product product);

    void Delete(Product product);
}
