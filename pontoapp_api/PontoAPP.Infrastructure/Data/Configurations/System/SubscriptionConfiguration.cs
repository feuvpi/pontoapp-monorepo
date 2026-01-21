using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Infrastructure.Data.Configurations.System;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions", "public");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.HasIndex(s => s.TenantId)
            .IsUnique();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<string>() // Armazena como string no banco
            .HasMaxLength(20);

        builder.Property(s => s.StartDate)
            .IsRequired();

        builder.Property(s => s.EndDate);

        builder.Property(s => s.TrialEndDate);

        builder.Property(s => s.MaxUsers)
            .IsRequired()
            .HasDefaultValue(10);

        builder.Property(s => s.MonthlyPrice)
            .IsRequired()
            .HasColumnType("decimal(10,2)")
            .HasDefaultValue(0);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Índice para queries por status
        builder.HasIndex(s => s.Status);

        // Índice para verificar trials expirados
        builder.HasIndex(s => s.TrialEndDate);
    }
}