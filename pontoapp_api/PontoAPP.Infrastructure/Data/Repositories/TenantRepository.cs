using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Data.Repositories;

/// <summary>
/// Repository para Tenants (SystemDbContext)
/// </summary>
public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
{
    public TenantRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Slug == slug.ToLower(), cancellationToken);
    }

    public async Task<Tenant?> GetByIdWithSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(t => t.Subscription)
            .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(t => t.Slug == slug.ToLower(), cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        email = email.ToLowerInvariant();
        return await _dbSet
            .AnyAsync(t => t.Email.Value == email, cancellationToken);
    }

    public async Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.IsActive)
            .Include(t => t.Subscription)
            .ToListAsync(cancellationToken);
    }
}