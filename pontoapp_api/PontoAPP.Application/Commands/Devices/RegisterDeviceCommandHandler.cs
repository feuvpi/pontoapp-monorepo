using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Device;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.Devices;

public class RegisterDeviceCommandHandler(
    IDeviceRepository deviceRepository,
    IUserRepository userRepository,
    ILogger<RegisterDeviceCommandHandler> logger)
    : IRequestHandler<RegisterDeviceCommand, DeviceResponse>
{
    // Limite de devices por usuário
    private const int MaxDevicesPerUser = 3;

    public async Task<DeviceResponse> Handle(RegisterDeviceCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering device {DeviceId} for user {UserId}", request.DeviceId, request.UserId);

        // Validar usuário
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        // Verificar se device já existe para este usuário
        var existingDevice = await deviceRepository.GetByDeviceIdAndUserIdAsync(
            request.DeviceId, request.UserId, cancellationToken);

        if (existingDevice != null)
        {
            // Atualizar device existente
            existingDevice.UpdateInfo(request.Model, request.OsVersion, request.BiometricCapable);
            existingDevice.SetPushToken(request.PushToken);
            existingDevice.Activate();
            existingDevice.UpdateLastUsed();

            deviceRepository.Update(existingDevice);
            await deviceRepository.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Device updated: {DeviceId}", request.DeviceId);

            return MapToResponse(existingDevice);
        }

        // Verificar se device está registrado para outro usuário
        var deviceForOtherUser = await deviceRepository.GetByDeviceIdAsync(request.DeviceId, cancellationToken);
        if (deviceForOtherUser != null && deviceForOtherUser.UserId != request.UserId)
        {
            throw new BusinessException("Este dispositivo já está registrado para outro usuário.");
        }

        // Verificar limite de devices
        var activeDeviceCount = await deviceRepository.GetActiveDeviceCountByUserIdAsync(request.UserId, cancellationToken);
        if (activeDeviceCount >= MaxDevicesPerUser)
        {
            throw new BusinessException($"Limite de {MaxDevicesPerUser} dispositivos atingido. Desative um dispositivo antes de registrar outro.");
        }

        // Criar novo device
        var device = Device.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            deviceId: request.DeviceId,
            platform: request.Platform,
            model: request.Model,
            osVersion: request.OsVersion,
            biometricCapable: request.BiometricCapable
        );

        device.SetPushToken(request.PushToken);

        await deviceRepository.AddAsync(device, cancellationToken);
        await deviceRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Device registered: {DeviceId} for user {UserId}", device.DeviceId, device.UserId);

        return MapToResponse(device);
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