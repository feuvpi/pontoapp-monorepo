using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Domain.Repositories;

/// <summary>
/// Repository específico para Tenants (SystemDbContext)
/// </summary>
public interface ITenantRepository : IGenericRepository<Tenant>
{
    Task<Tenant?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdWithSubscriptionAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tenant>> GetActiveTenantsAsync(CancellationToken cancellationToken = default);
}