using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FirstName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.LastName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(a => a.Biography)
            .HasMaxLength(3000);

        builder.Property(a => a.Email)
            .HasMaxLength(256);

        builder.Property(a => a.Website)
            .HasMaxLength(500);

        builder.Property(a => a.Nationality)
            .HasMaxLength(100);

        builder.Ignore(a => a.FullName);

        builder.HasIndex(a => new { a.FirstName, a.LastName });

        builder.Property(a => a.CreatedAt).IsRequired();
        builder.Property(a => a.CreatedBy).HasMaxLength(256);
        builder.Property(a => a.UpdatedBy).HasMaxLength(256);
    }
}
