using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Items;
using ProductInventory.Application.DTOs.Products;

namespace ProductInventory.Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetPagedAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        string currentUser,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        string currentUser,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateQuantityAsync(
        int productId,
        UpdateItemQuantityRequest request,
        string currentUser,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}
