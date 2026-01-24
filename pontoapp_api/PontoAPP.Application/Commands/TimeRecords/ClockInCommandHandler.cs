using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;

namespace PontoAPP.Application.Commands.TimeRecords;

public class ClockInCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    IDeviceRepository deviceRepository,
    INSRGenerator nsrGenerator,
    ISignatureGenerator signatureGenerator,
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

        // Validar dispositivo
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

        // ========== PORTARIA 671: NSR + HASH ==========
        
        // 1. Gerar NSR (atômico)
        var nsr = await nsrGenerator.GenerateNextAsync(request.TenantId, cancellationToken);
        logger.LogInformation("Generated NSR {NSR} for user {UserId}", nsr, request.UserId);

        // 2. Gerar hash SHA-256
        var hash = signatureGenerator.GenerateHash(
            nsr: nsr,
            tenantId: request.TenantId,
            userId: user.Id,
            cpf: user.CPF.Value,
            recordedAt: DateTime.UtcNow,
            recordType: RecordType.ClockIn.ToString());

        // ========== FIM PORTARIA 671 ==========

        // Criar registro
        var timeRecord = TimeRecord.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            nsr: nsr,                         // ← NSR
            type: RecordType.ClockIn,
            //authenticationType: authType,
            signatureHash: hash,              // ← Hash
            latitude: request.Latitude,
            longitude: request.Longitude,
            ipAddress: request.IpAddress,     // ← Opcional
            userAgent: request.UserAgent,     // ← Opcional
            deviceId: device?.Id,             // ← DeviceId
            notes: request.Notes
        );

        await timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Clock-in registered: {RecordId} (NSR: {NSR}) for user {UserId} from device {DeviceId}", 
            timeRecord.Id, nsr, request.UserId, request.DeviceId);

        return new TimeRecordResponse
        {
            Id = timeRecord.Id,
            UserId = timeRecord.UserId,
            UserName = user.FullName,
            NSR = nsr,                        // ← Incluir NSR no response
            RecordedAt = timeRecord.RecordedAt,
            Type = timeRecord.Type.ToString(),
            Status = timeRecord.Status.ToString(),
            AuthenticationType = timeRecord.AuthenticationType.ToString(),
            SignatureHash = hash,             // ← Incluir hash no response
            Latitude = timeRecord.Latitude,
            Longitude = timeRecord.Longitude,
            Notes = timeRecord.Notes,
            CreatedAt = timeRecord.CreatedAt
        };
    }
}