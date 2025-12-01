using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Events;

/// <summary>
/// Evento disparado quando um registro de ponto é criado
/// Pode ser usado para validações, notificações, analytics, etc
/// </summary>
public class TimeRecordCreatedEvent
{
    public Guid TimeRecordId { get; }
    public Guid UserId { get; }
    public Guid TenantId { get; }
    public RecordType RecordType { get; }
    public DateTime RecordedAt { get; }
    public AuthenticationType AuthenticationType { get; }
    public DateTime OccurredAt { get; }

    public TimeRecordCreatedEvent(
        Guid timeRecordId,
        Guid userId,
        Guid tenantId,
        RecordType recordType,
        DateTime recordedAt,
        AuthenticationType authenticationType)
    {
        TimeRecordId = timeRecordId;
        UserId = userId;
        TenantId = tenantId;
        RecordType = recordType;
        RecordedAt = recordedAt;
        AuthenticationType = authenticationType;
        OccurredAt = DateTime.UtcNow;
    }
}