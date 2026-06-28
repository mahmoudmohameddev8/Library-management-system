using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class BorrowingTransactionConfiguration : IEntityTypeConfiguration<BorrowingTransaction>
{
    public void Configure(EntityTypeBuilder<BorrowingTransaction> builder)
    {
        builder.ToTable("BorrowingTransactions");

        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.BorrowDate)
            .IsRequired();

        builder.Property(bt => bt.DueDate)
            .IsRequired();

        builder.Property(bt => bt.ReturnDate)
            .IsRequired(false);

        builder.Property(bt => bt.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(BorrowingStatus.Active);

        builder.Property(bt => bt.Notes)
            .HasMaxLength(1000);

        builder.Property(bt => bt.ReturnProcessedByUserId)
            .IsRequired(false);

        builder.Ignore(bt => bt.IsOverdue);

        builder.HasOne(bt => bt.Book)
            .WithMany(b => b.BorrowingTransactions)
            .HasForeignKey(bt => bt.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bt => bt.Member)
            .WithMany(m => m.BorrowingTransactions)
            .HasForeignKey(bt => bt.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bt => bt.ProcessedByUser)
            .WithMany(u => u.ProcessedTransactions)
            .HasForeignKey(bt => bt.ProcessedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bt => bt.ReturnProcessedByUser)
            .WithMany()
            .HasForeignKey(bt => bt.ReturnProcessedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.HasIndex(bt => bt.BookId);
        builder.HasIndex(bt => bt.MemberId);
        builder.HasIndex(bt => bt.Status);
        builder.HasIndex(bt => bt.DueDate);

        // Composite index for the active-borrow lookup used to prevent double-borrowing
        builder.HasIndex(bt => new { bt.BookId, bt.MemberId, bt.Status })
            .HasDatabaseName("IX_BorrowingTransactions_BookMemberStatus");

        builder.Property(bt => bt.CreatedAt).IsRequired();
        builder.Property(bt => bt.CreatedBy).HasMaxLength(256);
        builder.Property(bt => bt.UpdatedBy).HasMaxLength(256);
    }
}
