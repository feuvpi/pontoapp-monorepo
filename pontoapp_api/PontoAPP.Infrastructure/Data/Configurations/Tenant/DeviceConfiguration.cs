using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Devices;

namespace PontoAPP.Infrastructure.Data.Configurations.Tenant;

public class DeviceConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("devices");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.DeviceId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Platform)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(d => d.Model)
            .HasMaxLength(100);

        builder.Property(d => d.OsVersion)
            .HasMaxLength(50);

        builder.Property(d => d.PushToken)
            .HasMaxLength(500);

        builder.Property(d => d.IsActive)
            .HasDefaultValue(true);

        builder.Property(d => d.CreatedBy)
            .HasMaxLength(100);

        builder.Property(d => d.UpdatedBy)
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(d => d.DeviceId);
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => new { d.DeviceId, d.UserId }).IsUnique();
        builder.HasIndex(d => d.TenantId);

        // Relationships
        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}