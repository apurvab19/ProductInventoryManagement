namespace ProductInventory.Domain.Entities;

public class RefreshToken
{
    public int Id { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime CreatedOnUtc { get; set; }

    public DateTime ExpiresOnUtc { get; set; }

    public DateTime? RevokedOnUtc { get; set; }

    public int UserId { get; set; }

    public AppUser User { get; set; } = null!;

    public bool IsActive =>
        RevokedOnUtc is null && ExpiresOnUtc > DateTime.UtcNow;
}
