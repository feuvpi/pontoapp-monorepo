using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Infrastructure.Data.Configurations.System;

public class TenantNSRCounterConfiguration : IEntityTypeConfiguration<TenantNSRCounter>
{
    public void Configure(EntityTypeBuilder<TenantNSRCounter> builder)
    {
        builder.ToTable("TenantNSRCounters", "public");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.CurrentNSR)
            .IsRequired()
            .HasDefaultValue(0L);

        builder.Property(c => c.UpdatedAt)
            .IsRequired();

        builder.HasIndex(c => c.TenantId)
            .IsUnique();

        builder.HasOne(c => c.Tenant)
            .WithMany()
            .HasForeignKey(c => c.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}