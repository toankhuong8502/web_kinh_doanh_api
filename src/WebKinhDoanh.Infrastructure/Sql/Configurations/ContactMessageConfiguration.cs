using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebKinhDoanh.Domain.Entities;

namespace WebKinhDoanh.Infrastructure.Sql.Configurations;

public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");
        builder.HasKey(message => message.Id);
        builder.Property(message => message.Name).HasMaxLength(120).IsRequired();
        builder.Property(message => message.Email).HasMaxLength(254).IsRequired();
        builder.Property(message => message.Phone).HasMaxLength(20);
        builder.Property(message => message.Subject).HasMaxLength(200).IsRequired();
        builder.Property(message => message.Message).HasMaxLength(5000).IsRequired();
    }
}
