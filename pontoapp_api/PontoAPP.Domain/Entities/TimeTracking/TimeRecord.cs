using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Entities.TimeTracking;

/// <summary>
/// Representa um registro de ponto (entrada ou saída)
/// IMUTÁVEL conforme Portaria 671 - não pode ser editado após criação
/// Ajustes são feitos via TimeRecordAdjustment
/// </summary>
public class TimeRecord : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; private set; }
    
    // NOVO: NSR - Número Sequencial de Registro (Portaria 671)
    public long NSR { get; private set; } // Sequencial único por tenant
    
    public DateTime RecordedAt { get; private set; }
    public RecordType Type { get; private set; }
    public RecordStatus Status { get; private set; }
    public AuthenticationType AuthenticationType { get; private set; }
    
    // Localização (opcional)
    public double? Latitude { get; private set; }
    public double? Longitude { get; private set; }
    
    // NOVO: Rastreabilidade - Portaria 671
    public string? IpAddress { get; private set; } // IP de origem da batida
    public string? UserAgent { get; private set; } // Navegador/App usado
    public Guid? DeviceId { get; private set; } // Dispositivo usado (se aplicável)
    
    // NOVO: Assinatura Digital - Portaria 671
    public string SignatureHash { get; private set; } = string.Empty; // SHA-256 do registro
    
    // NOVO: Marcador de ajuste
    public bool IsAdjustment { get; private set; } // Se true, é um ajuste de outro registro
    public Guid? OriginalTimeRecordId { get; private set; } // Se for ajuste, aponta para o original
    
    // Observações
    public string? Notes { get; private set; }
    
    // DEPRECADOS - Manter por compatibilidade mas não usar
    // [Obsolete("Use TimeRecordAdjustment para ajustes - Portaria 671")]
    // public string? EditReason { get; private set; }
    
    // [Obsolete("Use TimeRecordAdjustment para ajustes - Portaria 671")]
    // public DateTime? EditedAt { get; private set; }
    
    // [Obsolete("Use TimeRecordAdjustment para ajustes - Portaria 671")]
    // public string? EditedBy { get; set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Relacionamento
    public virtual User User { get; private set; } = null!;
    public virtual TimeRecord? OriginalTimeRecord { get; private set; } // Se for ajuste

    // EF Constructor
    private TimeRecord() { }

    private TimeRecord(
        Guid tenantId,
        Guid userId,
        long nsr,
        RecordType type,
        //AuthenticationType authenticationType,
        string signatureHash,
        double? latitude = null,
        double? longitude = null,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? deviceId = null,
        string? notes = null,
        bool isAdjustment = false,
        Guid? originalTimeRecordId = null)
    {
        TenantId = tenantId;
        UserId = userId;
        NSR = nsr;
        RecordedAt = DateTime.UtcNow;
        Type = type;
        Status = RecordStatus.Valid;
        //AuthenticationType = authenticationType;
        SignatureHash = signatureHash;
        Latitude = latitude;
        Longitude = longitude;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        DeviceId = deviceId;
        Notes = notes;
        IsAdjustment = isAdjustment;
        OriginalTimeRecordId = originalTimeRecordId;
    }

    /// <summary>
    /// Cria um novo registro de ponto
    /// </summary>
    public static TimeRecord Create(
        Guid tenantId,
        Guid userId,
        long nsr,
        RecordType type,
        //AuthenticationType authenticationType,
        string signatureHash,
        double? latitude = null,
        double? longitude = null,
        string? ipAddress = null,
        string? userAgent = null,
        Guid? deviceId = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHash))
            throw new ArgumentException("Signature hash is required", nameof(signatureHash));

        return new TimeRecord(
            tenantId, 
            userId, 
            nsr, 
            type, 
            signatureHash,
            latitude, 
            longitude, 
            ipAddress, 
            userAgent, 
            deviceId, 
            notes,
            isAdjustment: false,
            originalTimeRecordId: null);
    }

    /// <summary>
    /// Cria um registro de ajuste (correção de outro registro)
    /// </summary>
    public static TimeRecord CreateAdjustment(
        Guid tenantId,
        Guid userId,
        long nsr,
        RecordType type,
        DateTime recordedAt,
        AuthenticationType authenticationType,
        string signatureHash,
        Guid originalTimeRecordId,
        double? latitude = null,
        double? longitude = null,
        string? ipAddress = null,
        string? userAgent = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(signatureHash))
            throw new ArgumentException("Signature hash is required", nameof(signatureHash));

        var record = new TimeRecord(
            tenantId,
            userId,
            nsr,
            type,
            //authenticationType,
            signatureHash,
            latitude,
            longitude,
            ipAddress,
            userAgent,
            deviceId: null,
            notes,
            isAdjustment: true,
            originalTimeRecordId: originalTimeRecordId);

        // Para ajustes, permitimos setar RecordedAt manualmente
        record.RecordedAt = recordedAt;

        return record;
    }

    /// <summary>
    /// Altera status (único campo editável)
    /// </summary>
    public void SetStatus(RecordStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Rejeita um registro pendente
    /// </summary>
    public void Reject(string? reason = null)
    {
        if (Status != RecordStatus.Pending)
            throw new InvalidOperationException("Only pending records can be rejected");

        Status = RecordStatus.Rejected;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = reason;
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Aprova um registro pendente
    /// </summary>
    public void Approve()
    {
        if (Status == RecordStatus.Pending)
        {
            Status = RecordStatus.Valid;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Adiciona notas (única edição permitida)
    /// </summary>
    public void AddNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    // ==========================================
    // MÉTODOS DEPRECADOS - NÃO USAR
    // ==========================================

    // [Obsolete("TimeRecords are IMMUTABLE per Portaria 671. Use TimeRecordAdjustment instead.")]
    // public void UpdateRecordedAt(DateTime recordedAt)
    // {
    //     throw new InvalidOperationException(
    //         "TimeRecords cannot be edited after creation (Portaria 671). " +
    //         "Please create a TimeRecordAdjustment instead.");
    // }
    //
    // [Obsolete("TimeRecords are IMMUTABLE per Portaria 671. Use TimeRecordAdjustment instead.")]
    // public void UpdateType(RecordType type)
    // {
    //     throw new InvalidOperationException(
    //         "TimeRecords cannot be edited after creation (Portaria 671). " +
    //         "Please create a TimeRecordAdjustment instead.");
    // }
    //
    // [Obsolete("TimeRecords are IMMUTABLE per Portaria 671. Use TimeRecordAdjustment instead.")]
    // public void Edit(DateTime newRecordedAt, string editReason, string editedBy)
    // {
    //     throw new InvalidOperationException(
    //         "TimeRecords cannot be edited after creation (Portaria 671). " +
    //         "Please create a TimeRecordAdjustment instead.");
    // }
    //
    // [Obsolete("TimeRecords are IMMUTABLE per Portaria 671. Use TimeRecordAdjustment instead.")]
    // public void UpdateNotes(string? notes)
    // {
    //     throw new InvalidOperationException(
    //         "Use AddNotes() instead. Notes can only be added, not replaced.");
    // }
    //
    // [Obsolete("TimeRecords are IMMUTABLE per Portaria 671")]
    // public void UpdateLocation(double? latitude, double? longitude)
    // {
    //     throw new InvalidOperationException(
    //         "TimeRecords cannot be edited after creation (Portaria 671).");
    // }
}