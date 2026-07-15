using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Infrastructure.Data.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken");

        builder.HasKey(token => token.Id);

        builder.Property(token => token.TokenHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(token => token.CreatedOnUtc)
            .HasColumnType("datetime2");

        builder.Property(token => token.ExpiresOnUtc)
            .HasColumnType("datetime2");

        builder.Property(token => token.RevokedOnUtc)
            .HasColumnType("datetime2");

        builder.HasIndex(token => token.TokenHash)
            .IsUnique();

        builder.HasOne(token => token.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
