using MediatR;

namespace PontoAPP.Application.Commands.Devices;

public class DeactivateDeviceCommand : IRequest<bool>
{
    public Guid DeviceId { get; set; }
    public Guid UserId { get; set; }
}