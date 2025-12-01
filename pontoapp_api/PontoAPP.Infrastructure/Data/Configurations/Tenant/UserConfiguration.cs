using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Infrastructure.Data.Configurations.Tenant;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .ValueGeneratedNever();

        builder.Property(u => u.TenantId)
            .IsRequired();

        builder.HasIndex(u => u.TenantId);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        // Value Object Email
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(e => e.Value)
                .IsUnique();
        });

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Pin)
            .HasMaxLength(10);

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.BiometricHash)
            .HasMaxLength(500);

        builder.Property(u => u.EmployeeCode)
            .HasMaxLength(50);

        builder.HasIndex(u => u.EmployeeCode);

        builder.Property(u => u.Department)
            .HasMaxLength(100);

        builder.Property(u => u.HiredAt);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100);

        // Relacionamento com TimeRecords (1:N)
        builder.HasMany(u => u.TimeRecords)
            .WithOne(tr => tr.User)
            .HasForeignKey(tr => tr.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Não deletar user se tiver registros de ponto

        // Índices para queries comuns
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.Role);
    }
}