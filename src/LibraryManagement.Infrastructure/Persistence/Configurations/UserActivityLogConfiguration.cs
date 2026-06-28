using LibraryManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryManagement.Infrastructure.Persistence.Configurations;

public class UserActivityLogConfiguration : IEntityTypeConfiguration<UserActivityLog>
{
    public void Configure(EntityTypeBuilder<UserActivityLog> builder)
    {
        builder.ToTable("UserActivityLogs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.Action)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(l => l.EntityType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(l => l.EntityId)
            .HasMaxLength(100);

        builder.Property(l => l.OldValues)
            .HasColumnType("longtext");

        builder.Property(l => l.NewValues)
            .HasColumnType("longtext");

        builder.Property(l => l.IpAddress)
            .HasMaxLength(50);

        builder.Property(l => l.UserAgent)
            .HasMaxLength(500);

        builder.Property(l => l.Timestamp)
            .IsRequired();

        builder.Property(l => l.IsSuccess)
            .IsRequired();

        builder.Property(l => l.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(l => l.HttpStatusCode)
            .IsRequired();

        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Timestamp);
        builder.HasIndex(l => l.EntityType);
        builder.HasIndex(l => l.Action);
    }
}
