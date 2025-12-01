namespace PontoAPP.Application.DTOs.Device;

public class RegisterDeviceRequest
{
    public string DeviceId { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty; // iOS, Android
    public string? Model { get; set; }
    public string? OsVersion { get; set; }
    public bool BiometricCapable { get; set; }
    public string? PushToken { get; set; }
}