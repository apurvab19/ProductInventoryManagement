using ProductInventory.Domain.Common;

namespace ProductInventory.Domain.Entities;

public class Product : AuditableEntity
{
    public int Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public Item? Item { get; set; }
}
