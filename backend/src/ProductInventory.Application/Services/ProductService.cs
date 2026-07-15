using ProductInventory.Application.Common.Interfaces;
using ProductInventory.Application.Common.Models;
using ProductInventory.Application.DTOs.Items;
using ProductInventory.Application.DTOs.Products;
using ProductInventory.Application.Interfaces;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Application.Services;

public sealed class ProductService(IUnitOfWork unitOfWork)
    : IProductService
{
    public async Task<PagedResult<ProductDto>> GetPagedAsync(
        ProductQueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var result = await unitOfWork.Products.GetPagedAsync(
            parameters,
            cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = result.Items.Select(MapToDto).ToList(),
            PageNumber = result.PageNumber,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(
            id,
            cancellationToken);

        return product is null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductRequest request,
        string currentUser,
        CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            ProductName = request.ProductName.Trim(),
            CreatedBy = currentUser,
            CreatedOn = DateTime.UtcNow,
            Item = new Item
            {
                Quantity = request.Quantity
            }
        };

        await unitOfWork.Products.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        string currentUser,
        CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.ProductName = request.ProductName.Trim();
        product.ModifiedBy = currentUser;
        product.ModifiedOn = DateTime.UtcNow;

        unitOfWork.Products.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> UpdateQuantityAsync(
        int productId,
        UpdateItemQuantityRequest request,
        string currentUser,
        CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(
            productId,
            cancellationToken);

        if (product?.Item is null)
        {
            return false;
        }

        product.Item.Quantity = request.Quantity;
        product.ModifiedBy = currentUser;
        product.ModifiedOn = DateTime.UtcNow;

        unitOfWork.Products.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await unitOfWork.Products.GetByIdAsync(
            id,
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        unitOfWork.Products.Delete(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            ProductName = product.ProductName,
            CreatedBy = product.CreatedBy,
            CreatedOn = product.CreatedOn,
            ModifiedBy = product.ModifiedBy,
            ModifiedOn = product.ModifiedOn,
            Item = product.Item is null
                ? null
                : new ItemDto
                {
                    Id = product.Item.Id,
                    ProductId = product.Item.ProductId,
                    Quantity = product.Item.Quantity
                }
        };
    }
}

