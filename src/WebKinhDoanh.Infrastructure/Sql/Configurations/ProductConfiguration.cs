using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(product => product.Id);
        builder.Property(product => product.Name).HasMaxLength(200).IsRequired();
        builder.Property(product => product.Slug).HasMaxLength(220).IsRequired();
        builder.HasIndex(product => product.Slug).IsUnique();
        builder.Property(product => product.Description).HasMaxLength(2000).IsRequired();
        builder.Property(product => product.Detail).HasMaxLength(500).IsRequired();
        builder.Property(product => product.Price).HasPrecision(18, 2);
        builder.Property(product => product.OldPrice).HasPrecision(18, 2);
        builder.Property(product => product.Image).HasMaxLength(100).IsRequired();
        builder.Property(product => product.Badge).HasMaxLength(40);
        builder.HasIndex(product => new { product.CategoryId, product.IsActive });
    }
}
