namespace PontoAPP.Application.DTOs.Device;

public class DeviceResponse
{
    public Guid Id { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? OsVersion { get; set; }
    public bool BiometricCapable { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}