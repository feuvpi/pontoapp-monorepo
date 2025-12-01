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

        builder.Property(tr => tr.RecordedAt)
            .IsRequired();

        // Índice composto para queries de intervalo de data por usuário
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

        builder.Property(tr => tr.Notes)
            .HasMaxLength(500);

        builder.Property(tr => tr.EditReason)
            .HasMaxLength(500);

        builder.Property(tr => tr.EditedAt);

        builder.Property(tr => tr.EditedBy)
            .HasMaxLength(100);

        builder.Property(tr => tr.CreatedAt)
            .IsRequired();

        builder.Property(tr => tr.UpdatedAt);

        builder.Property(tr => tr.CreatedBy)
            .HasMaxLength(100);

        builder.Property(tr => tr.UpdatedBy)
            .HasMaxLength(100);

        // Índice para queries de relatórios por data
        builder.HasIndex(tr => tr.RecordedAt);

        // Índice composto para queries de registros pendentes por tenant
        builder.HasIndex(tr => new { tr.TenantId, tr.Status });
    }
}