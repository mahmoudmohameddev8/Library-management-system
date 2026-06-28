using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Website)
            .HasMaxLength(500);

        builder.Property(p => p.ContactEmail)
            .HasMaxLength(256);

        builder.Property(p => p.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(p => p.Address)
            .HasMaxLength(500);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Name);

        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.CreatedBy).HasMaxLength(256);
        builder.Property(p => p.UpdatedBy).HasMaxLength(256);
    }
}
