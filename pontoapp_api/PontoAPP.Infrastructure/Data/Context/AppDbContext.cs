using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Infrastructure.Data.Configurations.System;
using PontoAPP.Infrastructure.Data.Configurations.Tenant;
using PontoAPP.Infrastructure.Data.Interceptors;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Infrastructure.Data.Context;

/// <summary>
/// Contexto do banco de dados SYSTEM - contém informações globais
/// Schema: public
/// Contém: Tenants, Subscriptions
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ITenantAccessor? _tenantAccessor;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ITenantAccessor tenantAccessor) : base(options)
    {
        _tenantAccessor = tenantAccessor;
    }

    
    // System tables (sem filtro de tenant)
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    // Tenant tables (com filtro de tenant)
    public DbSet<User> Users => Set<User>();
    public DbSet<TimeRecord> TimeRecords => Set<TimeRecord>();
    public DbSet<Device> Devices => Set<Device>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define o schema padrão como 'public'
        modelBuilder.HasDefaultSchema("public");

        // Aplica as configurações das entidades
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceConfiguration()); 
        // Tenant configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TimeRecordConfiguration());
        
        // Global query filters para multi-tenancy
        var tenantId = _tenantAccessor?.GetTenantInfo()?.TenantId;
        
        if (tenantId.HasValue)
        {
            modelBuilder.Entity<User>()
                .HasQueryFilter(e => e.TenantId == tenantId.Value);

            modelBuilder.Entity<TimeRecord>()
                .HasQueryFilter(e => e.TenantId == tenantId.Value);
        }



    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        // Adiciona o interceptor de auditoria
        if (!optionsBuilder.IsConfigured)
        {
            return;
        }

        // O interceptor será adicionado via DI no Program.cs
    }
}