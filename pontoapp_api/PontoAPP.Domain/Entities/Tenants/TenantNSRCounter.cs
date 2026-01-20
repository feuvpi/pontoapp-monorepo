using PontoAPP.Domain.Entities.Common;

namespace PontoAPP.Domain.Entities.Tenants;

/// <summary>
/// Controla o contador de NSR (Número Sequencial de Registro) por tenant
/// Usado para garantir sequência única e crescente conforme Portaria 671
/// </summary>
public class TenantNSRCounter : BaseEntity
{
    public Guid TenantId { get; private set; }
    public long CurrentNSR { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Relacionamento
    public virtual Tenant Tenant { get; private set; } = null!;

    // EF Constructor
    private TenantNSRCounter() { }

    private TenantNSRCounter(Guid tenantId, long currentNSR = 0)
    {
        TenantId = tenantId;
        CurrentNSR = currentNSR;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cria contador inicial para um tenant
    /// </summary>
    public static TenantNSRCounter Create(Guid tenantId)
    {
        return new TenantNSRCounter(tenantId, currentNSR: 0);
    }

    /// <summary>
    /// Incrementa e retorna o próximo NSR
    /// ATENÇÃO: Este método deve ser chamado apenas pela stored procedure!
    /// Usar diretamente pode causar race conditions.
    /// </summary>
    [Obsolete("Do not call directly. Use NSRService.GetNextNSRAsync() which uses stored procedure.")]
    public long GetNextNSR()
    {
        CurrentNSR++;
        UpdatedAt = DateTime.UtcNow;
        return CurrentNSR;
    }

    /// <summary>
    /// Reseta contador (apenas para testes/migração)
    /// </summary>
    [Obsolete("Only for testing or data migration. Never use in production.")]
    public void Reset()
    {
        CurrentNSR = 0;
        UpdatedAt = DateTime.UtcNow;
    }
}