using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Entities.TimeTracking;

/// <summary>
/// Representa um registro de ponto (entrada ou saída)
/// Armazenada no TenantDbContext (schema específico do tenant)
/// </summary>
public class TimeRecord : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; private set; }
    public DateTime RecordedAt { get; private set; }
    public RecordType Type { get; private set; }
    public RecordStatus Status { get; private set; }
    public AuthenticationType AuthenticationType { get; private set; }
    
    // Localização (opcional)
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    
    // Observações e auditoria
    public string? Notes { get; private set; }
    public string? EditReason { get; private set; } // Motivo se foi editado
    public DateTime? EditedAt { get; private set; }
    public string? EditedBy { get; set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Relacionamento
    public virtual User User { get; private set; }

    // EF Constructor
    private TimeRecord() { }

    private TimeRecord(
        Guid tenantId,
        Guid userId,
        RecordType type,
        AuthenticationType authenticationType,
        double? latitude = null,
        double? longitude = null,
        string? notes = null)
    {
        TenantId = tenantId;
        UserId = userId;
        RecordedAt = DateTime.UtcNow;
        Type = type;
        Status = RecordStatus.Valid;
        AuthenticationType = authenticationType;
        Latitude = latitude;
        Longitude = longitude;
        Notes = notes;
    }

    public static TimeRecord Create(
        Guid tenantId,
        Guid userId,
        RecordType type,
        AuthenticationType authenticationType,
        double? latitude = null,
        double? longitude = null,
        string? notes = null)
    {
        return new TimeRecord(tenantId, userId, type, authenticationType, latitude, longitude, notes);
    }

    public void SetStatus(RecordStatus status)
    {
        Status = status;
    }
    
    public void UpdateRecordedAt(DateTime recordedAt)
    {
        RecordedAt = recordedAt;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateType(RecordType type)
    {
        Type = type;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Reject(string? reason = null)
    {
        Status = RecordStatus.Rejected;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = reason;
    }

    public void Approve()
    {
        if (Status == RecordStatus.Pending)
            Status = RecordStatus.Valid;
    }

    public void Edit(DateTime newRecordedAt, string editReason, string editedBy)
    {
        if (string.IsNullOrWhiteSpace(editReason))
            throw new ArgumentException("Edit reason is required", nameof(editReason));

        RecordedAt = newRecordedAt;
        Status = RecordStatus.Edited;
        EditReason = editReason;
        EditedAt = DateTime.UtcNow;
        EditedBy = editedBy;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }
    
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(double? latitude, double? longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
        UpdatedAt = DateTime.UtcNow;
    }
}