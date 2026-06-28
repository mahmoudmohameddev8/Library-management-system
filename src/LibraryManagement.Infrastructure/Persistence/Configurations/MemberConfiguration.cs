using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MembershipNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.FirstName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.LastName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(m => m.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(m => m.Address)
            .HasMaxLength(500);

        builder.Property(m => m.MembershipStartDate)
            .IsRequired();

        builder.Property(m => m.MembershipExpiryDate)
            .IsRequired();

        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50)
            .HasDefaultValue(MembershipStatus.Active);

        builder.Property(m => m.MaxBorrowLimit)
            .IsRequired()
            .HasDefaultValue(5);

        builder.Ignore(m => m.FullName);

        builder.HasIndex(m => m.MembershipNumber).IsUnique();
        builder.HasIndex(m => m.Email).IsUnique();
        builder.HasIndex(m => m.Status);

        builder.Property(m => m.CreatedAt).IsRequired();
        builder.Property(m => m.CreatedBy).HasMaxLength(256);
        builder.Property(m => m.UpdatedBy).HasMaxLength(256);
    }
}
