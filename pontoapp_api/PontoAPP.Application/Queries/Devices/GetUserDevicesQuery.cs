using MediatR;
using PontoAPP.Application.DTOs.Device;

namespace PontoAPP.Application.Queries.Devices;

public class GetUserDevicesQuery : IRequest<IEnumerable<DeviceResponse>>
{
    public Guid UserId { get; set; }
    public bool ActiveOnly { get; set; } = true;
}