using MediatR;

namespace PontoAPP.Domain.Events;

/// <summary>
/// Evento disparado quando um novo tenant é criado
/// Usado para provisionar schema e configurações iniciais
/// </summary>
public class TenantCreatedEvent : INotification
{
    public Guid TenantId { get; }
    public string TenantName { get; }
    public DateTime OccurredAt { get; }

    public TenantCreatedEvent(Guid tenantId, string tenantName)
    {
        TenantId = tenantId;
        TenantName = tenantName;
        OccurredAt = DateTime.UtcNow;
    }
}