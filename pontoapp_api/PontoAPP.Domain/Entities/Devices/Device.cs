// PontoAPP.Domain/Entities/Devices/Device.cs
using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;

namespace PontoAPP.Domain.Entities.Devices;

public class Device : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; private set; }
    public string DeviceId { get; private set; } = string.Empty; // UUID único do device
    public string Platform { get; private set; } = string.Empty; // iOS, Android
    public string? Model { get; private set; } // iPhone 15, Samsung S24
    public string? OsVersion { get; private set; } // iOS 17.1, Android 14
    public bool BiometricCapable { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastUsedAt { get; private set; }
    public string? PushToken { get; private set; } // Para notificações futuras
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Relacionamento
    public virtual User User { get; private set; } = null!;

    // EF Constructor
    private Device() { }

    private Device(
        Guid tenantId,
        Guid userId,
        string deviceId,
        string platform,
        string? model,
        string? osVersion,
        bool biometricCapable)
    {
        TenantId = tenantId;
        UserId = userId;
        DeviceId = deviceId;
        Platform = platform;
        Model = model;
        OsVersion = osVersion;
        BiometricCapable = biometricCapable;
        IsActive = true;
        LastUsedAt = DateTime.UtcNow;
    }

    public static Device Create(
        Guid tenantId,
        Guid userId,
        string deviceId,
        string platform,
        string? model = null,
        string? osVersion = null,
        bool biometricCapable = false)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("Device ID is required", nameof(deviceId));

        if (string.IsNullOrWhiteSpace(platform))
            throw new ArgumentException("Platform is required", nameof(platform));

        return new Device(tenantId, userId, deviceId, platform, model, osVersion, biometricCapable);
    }

    public void UpdateLastUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string? model, string? osVersion, bool? biometricCapable)
    {
        if (!string.IsNullOrWhiteSpace(model))
            Model = model;

        if (!string.IsNullOrWhiteSpace(osVersion))
            OsVersion = osVersion;

        if (biometricCapable.HasValue)
            BiometricCapable = biometricCapable.Value;
    }

    public void SetPushToken(string? pushToken)
    {
        PushToken = pushToken;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}