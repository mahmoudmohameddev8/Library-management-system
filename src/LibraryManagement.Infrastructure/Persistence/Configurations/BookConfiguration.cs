using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.ISBN)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("ISBN");

        builder.HasIndex(b => b.ISBN).IsUnique();

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(b => b.Description)
            .HasMaxLength(3000);

        builder.Property(b => b.TotalCopies)
            .IsRequired();

        builder.Property(b => b.AvailableCopies)
            .IsRequired();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(BookStatus.Available);

        builder.Property(b => b.CoverImageUrl)
            .HasMaxLength(1000);

        builder.Property(b => b.PageCount)
            .IsRequired(false);

        builder.HasOne(b => b.Publisher)
            .WithMany(p => p.Books)
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Language)
            .WithMany(l => l.Books)
            .HasForeignKey(b => b.LanguageId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.Title);
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.CategoryId);
        builder.HasIndex(b => b.PublisherId);

        builder.Property(b => b.CreatedAt).IsRequired();
        builder.Property(b => b.CreatedBy).HasMaxLength(256);
        builder.Property(b => b.UpdatedBy).HasMaxLength(256);
    }
}
