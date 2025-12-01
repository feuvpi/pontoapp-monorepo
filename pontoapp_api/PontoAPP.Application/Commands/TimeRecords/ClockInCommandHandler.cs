using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.TimeRecords;

public class ClockInCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    IDeviceRepository deviceRepository,
    ILogger<ClockInCommandHandler> logger)
    : IRequestHandler<ClockInCommand, TimeRecordResponse>
{
    private const int MinimumIntervalMinutes = 1;

    public async Task<TimeRecordResponse> Handle(ClockInCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Clock-in attempt for user: {UserId} from device: {DeviceId}", 
            request.UserId, request.DeviceId);

        // Validar usuário
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        if (!user.IsActive)
        {
            throw new BusinessException("Usuário inativo. Não é possível registrar ponto.");
        }

        // NOVO: Validar dispositivo
        if (string.IsNullOrWhiteSpace(request.DeviceId))
        {
            throw new ValidationException("Device ID é obrigatório.");
        }

        var isDeviceAuthorized = await deviceRepository.IsDeviceAuthorizedAsync(
            request.DeviceId, request.UserId, cancellationToken);

        if (!isDeviceAuthorized)
        {
            logger.LogWarning("Unauthorized device attempt: {DeviceId} for user {UserId}", 
                request.DeviceId, request.UserId);
            throw new BusinessException("Dispositivo não autorizado. Registre o dispositivo primeiro.");
        }

        // Atualizar último uso do device
        var device = await deviceRepository.GetByDeviceIdAndUserIdAsync(
            request.DeviceId, request.UserId, cancellationToken);
        
        if (device != null)
        {
            device.UpdateLastUsed();
            deviceRepository.Update(device);
        }

        // Validar se está tentando usar biometria em device sem suporte
        if (request.AuthenticationType.Equals("Biometric", StringComparison.OrdinalIgnoreCase) 
            && device?.BiometricCapable != true)
        {
            throw new BusinessException("Este dispositivo não suporta autenticação biométrica.");
        }

        // Validar último registro
        var lastRecord = await timeRecordRepository.GetLastRecordByUserIdAsync(request.UserId, cancellationToken);

        if (lastRecord != null)
        {
            if (lastRecord.Type == RecordType.ClockIn)
            {
                throw new BusinessException("Você já registrou entrada. Registre a saída primeiro.");
            }

            var timeSinceLastRecord = DateTime.UtcNow - lastRecord.RecordedAt;
            if (timeSinceLastRecord.TotalMinutes < MinimumIntervalMinutes)
            {
                throw new BusinessException($"Aguarde pelo menos {MinimumIntervalMinutes} minuto(s) entre registros.");
            }
        }

        // Validar tipo de autenticação
        if (!Enum.TryParse<AuthenticationType>(request.AuthenticationType, true, out var authType))
        {
            throw new ValidationException("Tipo de autenticação inválido.");
        }

        // Criar registro
        var timeRecord = TimeRecord.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            type: RecordType.ClockIn,
            authenticationType: authType,
            latitude: request.Latitude,
            longitude: request.Longitude,
            notes: request.Notes
        );

        await timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Clock-in registered: {RecordId} for user {UserId} from device {DeviceId}", 
            timeRecord.Id, request.UserId, request.DeviceId);

        return new TimeRecordResponse
        {
            Id = timeRecord.Id,
            UserId = timeRecord.UserId,
            UserName = user.FullName,
            RecordedAt = timeRecord.RecordedAt,
            Type = timeRecord.Type.ToString(),
            Status = timeRecord.Status.ToString(),
            AuthenticationType = timeRecord.AuthenticationType.ToString(),
            Latitude = timeRecord.Latitude,
            Longitude = timeRecord.Longitude,
            Notes = timeRecord.Notes,
            CreatedAt = timeRecord.CreatedAt
        };
    }
}