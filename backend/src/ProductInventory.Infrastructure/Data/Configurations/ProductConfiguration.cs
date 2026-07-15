using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductInventory.Domain.Entities;

namespace ProductInventory.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.HasKey(product => product.Id);

        builder.Property(product => product.Id)
            .ValueGeneratedOnAdd();

        builder.Property(product => product.ProductName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(product => product.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(product => product.CreatedOn)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(product => product.ModifiedBy)
            .HasMaxLength(100);

        builder.Property(product => product.ModifiedOn)
            .HasColumnType("datetime2");

        builder.HasIndex(product => product.ProductName);

        builder.HasOne(product => product.Item)
            .WithOne(item => item.Product)
            .HasForeignKey<Item>(item => item.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
