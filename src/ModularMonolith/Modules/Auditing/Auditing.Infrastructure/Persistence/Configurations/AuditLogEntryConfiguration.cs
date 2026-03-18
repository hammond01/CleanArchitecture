using Auditing.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auditing.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for AuditLogEntry
/// </summary>
public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
{
    public void Configure(EntityTypeBuilder<AuditLogEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Action)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ObjectId)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Log)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(x => x.CreatedDateTime).IsRequired();

        builder.Property(x => x.UpdatedDateTime);

        // Indexes
        builder.HasIndex(x => new { x.UserId, x.CreatedDateTime })
            .IsDescending(false, true);

        builder.HasIndex(x => x.Action);

        // Table name
        builder.ToTable("AuditLogEntries", "auditing");
    }
}

