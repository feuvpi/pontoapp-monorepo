using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Infrastructure.Data.Configurations.System;
using PontoAPP.Infrastructure.Data.Configurations.Tenant;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Infrastructure.Data.Context;

/// <summary>
/// Contexto do banco de dados - contém informações globais e tenant-specific
/// Schema: public
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
    public DbSet<TimeRecordAdjustment> TimeRecordAdjustments => Set<TimeRecordAdjustment>();
    public DbSet<TenantNSRCounter> TenantNSRCounters => Set<TenantNSRCounter>();
    public DbSet<WorkSchedule> WorkSchedules => Set<WorkSchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Define o schema padrão como 'public'
        modelBuilder.HasDefaultSchema("public");

        // Aplica as configurações das entidades
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new DeviceConfiguration()); 
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TimeRecordConfiguration());
        modelBuilder.ApplyConfiguration(new TenantNSRCounterConfiguration());
        
        // ✅ CORREÇÃO: Global query filters com expressão dinâmica
        // A expressão _tenantAccessor.GetTenantInfo() é avaliada em RUNTIME, não em build time
        
        modelBuilder.Entity<User>()
            .HasQueryFilter(e => 
                _tenantAccessor == null || 
                _tenantAccessor.GetTenantInfo() == null || 
                e.TenantId == _tenantAccessor.GetTenantInfo()!.TenantId);

        modelBuilder.Entity<TimeRecord>()
            .HasQueryFilter(e => 
                _tenantAccessor == null || 
                _tenantAccessor.GetTenantInfo() == null || 
                e.TenantId == _tenantAccessor.GetTenantInfo()!.TenantId);
                
        modelBuilder.Entity<Device>()
            .HasQueryFilter(e => 
                _tenantAccessor == null || 
                _tenantAccessor.GetTenantInfo() == null || 
                e.TenantId == _tenantAccessor.GetTenantInfo()!.TenantId);
                
        modelBuilder.Entity<TimeRecordAdjustment>()
            .HasQueryFilter(e => 
                _tenantAccessor == null || 
                _tenantAccessor.GetTenantInfo() == null || 
                e.TenantId == _tenantAccessor.GetTenantInfo()!.TenantId);
                
        modelBuilder.Entity<WorkSchedule>()
            .HasQueryFilter(e => 
                _tenantAccessor == null || 
                _tenantAccessor.GetTenantInfo() == null || 
                e.TenantId == _tenantAccessor.GetTenantInfo()!.TenantId);
    }
}