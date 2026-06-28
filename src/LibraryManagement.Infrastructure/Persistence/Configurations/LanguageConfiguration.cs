using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.ToTable("Languages");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(l => l.Code).IsUnique();
        builder.HasIndex(l => l.Name).IsUnique();

        builder.Property(l => l.CreatedAt).IsRequired();
        builder.Property(l => l.CreatedBy).HasMaxLength(256);
        builder.Property(l => l.UpdatedBy).HasMaxLength(256);
    }
}
