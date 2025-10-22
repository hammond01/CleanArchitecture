using IdentityServer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityServer.Infrastructure.Data.Configurations;

public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.RefreshToken)
            .HasMaxLength(500);

        builder.Property(s => s.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(s => s.UserAgent)
            .HasMaxLength(500);

        builder.Property(s => s.DeviceInfo)
            .HasMaxLength(200);

        builder.Property(s => s.RevokedReason)
            .HasMaxLength(500);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.RefreshToken);
        builder.HasIndex(s => s.IsActive);
        builder.HasIndex(s => s.CreatedAt);
        builder.HasIndex(s => s.RefreshTokenExpiresAt);
    }
}
