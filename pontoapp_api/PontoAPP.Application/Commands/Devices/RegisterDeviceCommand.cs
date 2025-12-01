using MediatR;
using PontoAPP.Application.DTOs.Device;

namespace PontoAPP.Application.Commands.Devices;

public class RegisterDeviceCommand : IRequest<DeviceResponse>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string? Model { get; set; }
    public string? OsVersion { get; set; }
    public bool BiometricCapable { get; set; }
    public string? PushToken { get; set; }
}