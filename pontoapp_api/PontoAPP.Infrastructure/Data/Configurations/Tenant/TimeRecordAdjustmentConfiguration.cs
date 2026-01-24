using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.TimeTracking;

namespace PontoAPP.Infrastructure.Data.Configurations.Tenant;

public class TimeRecordAdjustmentConfiguration : IEntityTypeConfiguration<TimeRecordAdjustment>
{
    public void Configure(EntityTypeBuilder<TimeRecordAdjustment> builder)
    {
        builder.ToTable("time_record_adjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.OriginalRecordId)
            .IsRequired();

        builder.Property(a => a.AdjustmentRecordId);

        builder.Property(a => a.OriginalRecordedAt)
            .IsRequired();

        builder.Property(a => a.NewRecordedAt)
            .IsRequired();

        builder.Property(a => a.NewType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(a => a.Reason)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(a => a.RejectionReason)
            .HasColumnType("text");

        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.RequestedBy)
            .IsRequired();

        builder.Property(a => a.RequestedAt)
            .IsRequired();

        builder.Property(a => a.ApprovedBy);

        builder.Property(a => a.ApprovedAt);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.CreatedBy)
            .HasMaxLength(100);

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(100);

        builder.HasIndex(a => new { a.TenantId, a.Status })
            .HasDatabaseName("IX_adjustments_tenant_status");

        builder.HasIndex(a => a.OriginalRecordId)
            .HasDatabaseName("IX_adjustments_original_record");

        builder.HasIndex(a => a.RequestedBy)
            .HasDatabaseName("IX_adjustments_requested_by");

        builder.HasOne(a => a.OriginalRecord)
            .WithMany()
            .HasForeignKey(a => a.OriginalRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.AdjustmentRecord)
            .WithMany()
            .HasForeignKey(a => a.AdjustmentRecordId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Requester)
            .WithMany()
            .HasForeignKey(a => a.RequestedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Approver)
            .WithMany()
            .HasForeignKey(a => a.ApprovedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}