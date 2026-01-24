using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Infrastructure.Data.Configurations.Tenant;

public class TimeRecordConfiguration : IEntityTypeConfiguration<TimeRecord>
{
    public void Configure(EntityTypeBuilder<TimeRecord> builder)
    {
        builder.ToTable("time_records");

        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Id)
            .ValueGeneratedNever();

        builder.Property(tr => tr.TenantId)
            .IsRequired();

        builder.HasIndex(tr => tr.TenantId);

        builder.Property(tr => tr.UserId)
            .IsRequired();

        builder.HasIndex(tr => tr.UserId);

        // NOVO: NSR (Portaria 671)
        builder.Property(tr => tr.NSR)
            .IsRequired();

        // NOVO: SignatureHash (Portaria 671)
        builder.Property(tr => tr.SignatureHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(tr => tr.RecordedAt)
            .IsRequired();

        builder.HasIndex(tr => new { tr.UserId, tr.RecordedAt });

        builder.Property(tr => tr.Type)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(tr => tr.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(tr => tr.Status);

        builder.Property(tr => tr.AuthenticationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(tr => tr.Latitude)
            .HasColumnType("decimal(10,8)");

        builder.Property(tr => tr.Longitude)
            .HasColumnType("decimal(11,8)");

        // NOVO: Rastreabilidade (Portaria 671)
        builder.Property(tr => tr.IpAddress)
            .HasMaxLength(45);

        builder.Property(tr => tr.UserAgent)
            .HasColumnType("text");

        builder.Property(tr => tr.DeviceId);

        // NOVO: Campos de ajuste
        builder.Property(tr => tr.IsAdjustment)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(tr => tr.OriginalTimeRecordId);

        builder.Property(tr => tr.Notes)
            .HasColumnType("text");

        builder.Property(tr => tr.CreatedAt)
            .IsRequired();

        builder.Property(tr => tr.UpdatedAt);

        builder.Property(tr => tr.CreatedBy)
            .HasMaxLength(100);

        builder.Property(tr => tr.UpdatedBy)
            .HasMaxLength(100);

        // NOVO: Índice único NSR por tenant (Portaria 671)
        builder.HasIndex(tr => new { tr.TenantId, tr.NSR })
            .HasDatabaseName("IX_time_records_tenant_nsr")
            .IsUnique();

        // NOVO: Relacionamento com registro original (se for ajuste)
        builder.HasOne(tr => tr.OriginalTimeRecord)
            .WithMany()
            .HasForeignKey(tr => tr.OriginalTimeRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(tr => tr.RecordedAt);
        builder.HasIndex(tr => new { tr.TenantId, tr.Status });
    }
}