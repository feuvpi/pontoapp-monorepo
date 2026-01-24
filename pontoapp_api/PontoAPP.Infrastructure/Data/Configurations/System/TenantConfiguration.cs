using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Infrastructure.Data.Configurations.System;

public class TenantConfiguration : IEntityTypeConfiguration<Domain.Entities.Tenants.Tenant>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Tenants.Tenant> builder)
    
    {
        builder.ToTable("tenants", "public");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        // Value Object Email
        builder.OwnsOne(t => t.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);
        });

        // Value Object CNPJ - CORRIGIDO
        builder.OwnsOne(t => t.CNPJ, cnpj =>
        {
            cnpj.Property(c => c.Value)
                .HasColumnName("cnpj")
                .IsRequired()
                .HasMaxLength(14)
                .IsFixedLength();
            
            // Índice único dentro do OwnsOne
            cnpj.HasIndex(c => c.Value)
                .HasDatabaseName("IX_tenants_cnpj")
                .IsUnique();
        });

        builder.Property(t => t.CEI)
            .HasMaxLength(12)
            .IsFixedLength();

        builder.Property(t => t.InscricaoEstadual)
            .HasMaxLength(20);

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(100);

        builder.Property(t => t.UpdatedBy)
            .HasMaxLength(100);

    }
}