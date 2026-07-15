using Moq;
using ProductInventory.Application.Common.Interfaces;
using ProductInventory.Application.DTOs.Products;
using ProductInventory.Application.Services;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Application.Tests;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_CreatesProductAndInventoryItem()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var productRepository = new Mock<IProductRepository>();
        var itemRepository = new Mock<IItemRepository>();

        unitOfWork.SetupGet(x => x.Products)
            .Returns(productRepository.Object);

        unitOfWork.SetupGet(x => x.Items)
            .Returns(itemRepository.Object);

        unitOfWork.Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        productRepository.Setup(x => x.AddAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) =>
            {
                product.Id = 1;

                if (product.Item is not null)
                {
                    product.Item.Id = 1;
                    product.Item.ProductId = 1;
                }
            })
            .Returns(Task.CompletedTask);

        var service = new ProductService(unitOfWork.Object);

        var result = await service.CreateAsync(
            new CreateProductRequest
            {
                ProductName = "Laptop",
                Quantity = 10
            },
            "tester");

        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.ProductName);
        Assert.Equal("tester", result.CreatedBy);
        Assert.NotNull(result.Item);
        Assert.Equal(10, result.Item.Quantity);

        productRepository.Verify(x => x.AddAsync(
            It.IsAny<Product>(),
            It.IsAny<CancellationToken>()), Times.Once);

        unitOfWork.Verify(x => x.SaveChangesAsync(
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenProductDoesNotExist()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var productRepository = new Mock<IProductRepository>();

        unitOfWork.SetupGet(x => x.Products)
            .Returns(productRepository.Object);

        productRepository.Setup(x => x.GetByIdAsync(
                99,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(unitOfWork.Object);

        var result = await service.DeleteAsync(99);

        Assert.False(result);

        productRepository.Verify(
            x => x.Delete(It.IsAny<Product>()),
            Times.Never);
    }
}
