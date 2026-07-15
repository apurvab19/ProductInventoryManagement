namespace ProductInventory.Application.DTOs.Products;

public class CreateProductRequest
{
    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }
}
