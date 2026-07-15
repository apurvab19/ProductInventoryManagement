namespace ProductInventory.Application.DTOs.Products;

public class ProductQueryParameters
{
    public string? Search { get; set; }

    public string SortBy { get; set; } = "productName";

    public string SortDirection { get; set; } = "asc";

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}
