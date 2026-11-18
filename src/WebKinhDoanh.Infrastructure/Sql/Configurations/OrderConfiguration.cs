using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(order => order.Id);
        builder.Property(order => order.OrderNumber).HasMaxLength(32).IsRequired();
        builder.HasIndex(order => order.OrderNumber).IsUnique();
        builder.Property(order => order.CustomerName).HasMaxLength(120).IsRequired();
        builder.Property(order => order.Phone).HasMaxLength(20).IsRequired();
        builder.Property(order => order.Email).HasMaxLength(254);
        builder.Property(order => order.Address).HasMaxLength(300).IsRequired();
        builder.Property(order => order.Note).HasMaxLength(1000);
        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(order => order.Subtotal).HasPrecision(18, 2);
        builder.HasMany(order => order.Items)
            .WithOne(item => item.Order)
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
