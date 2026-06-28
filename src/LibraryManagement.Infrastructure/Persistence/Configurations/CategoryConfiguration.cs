using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.ParentCategoryId)
            .IsRequired(false);

        // Self-referencing relationship for hierarchical categories
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.ParentCategoryId);

        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.CreatedBy).HasMaxLength(256);
        builder.Property(c => c.UpdatedBy).HasMaxLength(256);
    }
}
