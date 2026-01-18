using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Entities.TimeTracking;

/// <summary>
/// Representa uma solicitação de ajuste de registro de ponto
/// Usado quando precisa corrigir um TimeRecord sem deletá-lo (Portaria 671)
/// </summary>
public class TimeRecordAdjustment : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    
    // Registro original que precisa de ajuste
    public Guid OriginalRecordId { get; private set; }
    
    // Novo registro criado (preenchido após aprovação)
    public Guid? AdjustmentRecordId { get; private set; }
    
    // Dados da solicitação
    public DateTime OriginalRecordedAt { get; private set; }
    public DateTime NewRecordedAt { get; private set; }
    public RecordType? NewType { get; private set; } // Se mudou o tipo também
    public string Reason { get; private set; } = string.Empty;
    
    // Fluxo de aprovação
    public AdjustmentStatus Status { get; private set; }
    public Guid RequestedBy { get; private set; }
    public DateTime RequestedAt { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Relacionamentos
    public virtual TimeRecord OriginalRecord { get; private set; } = null!;
    public virtual TimeRecord? AdjustmentRecord { get; private set; }
    public virtual User Requester { get; private set; } = null!;
    public virtual User? Approver { get; private set; }

    // EF Constructor
    private TimeRecordAdjustment() { }

    private TimeRecordAdjustment(
        Guid tenantId,
        Guid originalRecordId,
        DateTime originalRecordedAt,
        DateTime newRecordedAt,
        RecordType? newType,
        string reason,
        Guid requestedBy)
    {
        TenantId = tenantId;
        OriginalRecordId = originalRecordId;
        OriginalRecordedAt = originalRecordedAt;
        NewRecordedAt = newRecordedAt;
        NewType = newType;
        Reason = reason;
        RequestedBy = requestedBy;
        RequestedAt = DateTime.UtcNow;
        Status = AdjustmentStatus.Pending;
    }

    /// <summary>
    /// Cria solicitação de ajuste
    /// </summary>
    public static TimeRecordAdjustment Create(
        Guid tenantId,
        Guid originalRecordId,
        DateTime originalRecordedAt,
        DateTime newRecordedAt,
        RecordType? newType,
        string reason,
        Guid requestedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for adjustments", nameof(reason));

        if (reason.Length < 10)
            throw new ArgumentException("Reason must be at least 10 characters", nameof(reason));

        if (newRecordedAt > DateTime.UtcNow)
            throw new ArgumentException("Cannot adjust to future time", nameof(newRecordedAt));

        return new TimeRecordAdjustment(
            tenantId,
            originalRecordId,
            originalRecordedAt,
            newRecordedAt,
            newType,
            reason,
            requestedBy);
    }

    /// <summary>
    /// Aprova o ajuste
    /// </summary>
    public void Approve(Guid approvedBy, Guid adjustmentRecordId)
    {
        if (Status != AdjustmentStatus.Pending)
            throw new InvalidOperationException("Only pending adjustments can be approved");

        Status = AdjustmentStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovedAt = DateTime.UtcNow;
        AdjustmentRecordId = adjustmentRecordId;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejeita o ajuste
    /// </summary>
    public void Reject(Guid rejectedBy, string rejectionReason)
    {
        if (Status != AdjustmentStatus.Pending)
            throw new InvalidOperationException("Only pending adjustments can be rejected");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required", nameof(rejectionReason));

        Status = AdjustmentStatus.Rejected;
        ApprovedBy = rejectedBy;
        ApprovedAt = DateTime.UtcNow;
        RejectionReason = rejectionReason;
        UpdatedAt = DateTime.UtcNow;
    }
}
