namespace PontoAPP.Domain.Entities.Common;

/// <summary>
/// Interface para entidades que pertencem a um tenant específico
/// Todas as entidades no TenantDbContext devem implementar esta interface
/// </summary>
public interface ITenantEntity
{
    Guid TenantId { get; set; }
}