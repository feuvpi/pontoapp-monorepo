using MediatR;
using PontoAPP.Application.DTOs.Device;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.Devices;

public class GetUserDevicesQueryHandler(IDeviceRepository deviceRepository)
    : IRequestHandler<GetUserDevicesQuery, IEnumerable<DeviceResponse>>
{
    public async Task<IEnumerable<DeviceResponse>> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        var devices = request.ActiveOnly
            ? await deviceRepository.GetActiveByUserIdAsync(request.UserId, cancellationToken)
            : await deviceRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return devices.Select(MapToResponse);
    }

    private static DeviceResponse MapToResponse(Device device)
    {
        return new DeviceResponse
        {
            Id = device.Id,
            DeviceId = device.DeviceId,
            Platform = device.Platform,
            Model = device.Model,
            OsVersion = device.OsVersion,
            BiometricCapable = device.BiometricCapable,
            IsActive = device.IsActive,
            LastUsedAt = device.LastUsedAt,
            CreatedAt = device.CreatedAt
        };
    }
}