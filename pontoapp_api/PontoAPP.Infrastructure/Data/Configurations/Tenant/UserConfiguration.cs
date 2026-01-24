using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PontoAPP.Domain.Entities.Identity;

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
            
            // Índice no email (dentro do OwnsOne)
            email.HasIndex(e => e.Value)
                .HasDatabaseName("IX_users_email");
        });

        // Value Object CPF
        builder.OwnsOne(u => u.CPF, cpf =>
        {
            cpf.Property(c => c.Value)
                .HasColumnName("cpf")
                .IsRequired()
                .HasMaxLength(11)
                .IsFixedLength();
            
            // Índice no CPF (dentro do OwnsOne)
            cpf.HasIndex(c => c.Value)
                .HasDatabaseName("IX_users_cpf");
        });

        // Value Object PIS (nullable)
        builder.OwnsOne(u => u.PIS, pis =>
        {
            pis.Property(p => p.Value)
                .HasColumnName("pis")
                .HasMaxLength(11)
                .IsFixedLength();
            
            // Índice no PIS (dentro do OwnsOne, com filtro para nulls)
            pis.HasIndex(p => p.Value)
                .HasDatabaseName("IX_users_pis")
                .HasFilter("\"pis\" IS NOT NULL");
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

        builder.Property(u => u.MustChangePassword)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.RefreshToken)
            .HasColumnType("text");

        builder.Property(u => u.RefreshTokenExpiresAt);

        builder.Property(u => u.LastLoginAt);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100);

        // Relacionamento com TimeRecords
        builder.HasMany(u => u.TimeRecords)
            .WithOne(tr => tr.User)
            .HasForeignKey(tr => tr.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices simples
        builder.HasIndex(u => u.IsActive);
        builder.HasIndex(u => u.Role);
    }
}