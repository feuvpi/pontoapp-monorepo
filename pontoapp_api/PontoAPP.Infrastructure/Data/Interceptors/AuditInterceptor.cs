using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PontoAPP.Domain.Entities.Common;

namespace PontoAPP.Infrastructure.Data.Interceptors;

/// <summary>
/// Interceptor que preenche automaticamente campos de auditoria (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public AuditInterceptor(IHttpContextAccessor? httpContextAccessor = null)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntities(DbContext? context)
    {
        if (context == null) return;

        var currentUser = GetCurrentUser();
        var utcNow = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // CreatedAt já é setado no construtor da BaseEntity, mas garantimos aqui
                if (entry.Entity.CreatedAt == default)
                {
                    entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = utcNow;
                }
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(BaseEntity.UpdatedAt)).CurrentValue = utcNow;
            }
        }

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedBy = currentUser;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedBy = currentUser;
            }
        }
    }

    private string? GetCurrentUser()
    {
        // Pega o ID do usuário logado dos claims do JWT
        var httpContext = _httpContextAccessor?.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userId = httpContext.User.FindFirst("sub")?.Value 
                      ?? httpContext.User.FindFirst("userId")?.Value;
            return userId;
        }

        return null;
    }
}