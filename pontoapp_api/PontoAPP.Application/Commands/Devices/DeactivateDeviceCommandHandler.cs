using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.Devices;

public class DeactivateDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    ILogger<DeactivateDeviceCommandHandler> logger)
    : IRequestHandler<DeactivateDeviceCommand, bool>
{
    public async Task<bool> Handle(DeactivateDeviceCommand request, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.GetByIdAsync(request.DeviceId, cancellationToken);

        if (device == null)
        {
            throw new NotFoundException("Dispositivo não encontrado.");
        }

        device.Deactivate();

        deviceRepository.Update(device);
        await deviceRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Device deactivated: {DeviceId}", device.DeviceId);

        return true;
    }
}