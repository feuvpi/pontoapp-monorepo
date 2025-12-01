using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PontoAPP.Domain.Entities.Common;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor que:
/// 1. Preenche automaticamente o TenantId em entidades novas
/// 2. Aplica filtro global por TenantId em queries
/// </summary>
public class TenantInterceptor : SaveChangesInterceptor
{
    private readonly ITenantAccessor _tenantAccessor;

    public TenantInterceptor(ITenantAccessor tenantAccessor)
    {
        _tenantAccessor = tenantAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetTenantId(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetTenantId(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetTenantId(DbContext? context)
    {
        if (context == null) return;

        var tenantInfo = _tenantAccessor.GetTenantInfo();
        if (tenantInfo == null) return;

        foreach (var entry in context.ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.TenantId = tenantInfo.TenantId;
            }
            
            // Previne que alguém tente modificar o TenantId de uma entidade existente
            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(ITenantEntity.TenantId)).IsModified = false;
            }
        }
    }
}