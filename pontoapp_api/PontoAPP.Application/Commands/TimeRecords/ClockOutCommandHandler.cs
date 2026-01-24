using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;

namespace PontoAPP.Application.Commands.TimeRecords;

public class ClockOutCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    IDeviceRepository deviceRepository,
    INSRGenerator nsrGenerator,
    ISignatureGenerator signatureGenerator,
    ILogger<ClockOutCommandHandler> logger)
    : IRequestHandler<ClockOutCommand, TimeRecordResponse>
{
    private const int MinimumIntervalMinutes = 1;

    public async Task<TimeRecordResponse> Handle(ClockOutCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Clock-out attempt for user: {UserId}", request.UserId);

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

        // Validar dispositivo (se fornecido)
        if (!string.IsNullOrWhiteSpace(request.DeviceId))
        {
            var isDeviceAuthorized = await deviceRepository.IsDeviceAuthorizedAsync(
                request.DeviceId, request.UserId, cancellationToken);

            if (!isDeviceAuthorized)
            {
                logger.LogWarning("Unauthorized device attempt: {DeviceId} for user {UserId}", 
                    request.DeviceId, request.UserId);
                throw new BusinessException("Dispositivo não autorizado.");
            }

            // Atualizar último uso do device
            var device = await deviceRepository.GetByDeviceIdAndUserIdAsync(
                request.DeviceId, request.UserId, cancellationToken);
            
            if (device != null)
            {
                device.UpdateLastUsed();
                deviceRepository.Update(device);
            }
        }

        // Validar último registro
        var lastRecord = await timeRecordRepository.GetLastRecordByUserIdAsync(request.UserId, cancellationToken);

        if (lastRecord == null)
        {
            throw new BusinessException("Nenhum registro de entrada encontrado. Registre a entrada primeiro.");
        }

        // Verificar se já está "fora" (último registro foi saída)
        if (lastRecord.Type == RecordType.ClockOut)
        {
            throw new BusinessException("Você já registrou saída. Registre a entrada primeiro.");
        }

        // Verificar intervalo mínimo entre registros
        var timeSinceLastRecord = DateTime.UtcNow - lastRecord.RecordedAt;
        if (timeSinceLastRecord.TotalMinutes < MinimumIntervalMinutes)
        {
            throw new BusinessException($"Aguarde pelo menos {MinimumIntervalMinutes} minuto(s) entre registros.");
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
            recordType: RecordType.ClockOut.ToString());

        // ========== FIM PORTARIA 671 ==========

        // Buscar device (se fornecido)
        Guid? deviceId = null;
        if (!string.IsNullOrWhiteSpace(request.DeviceId))
        {
            var device = await deviceRepository.GetByDeviceIdAndUserIdAsync(
                request.DeviceId, request.UserId, cancellationToken);
            deviceId = device?.Id;
        }

        // Criar registro
        var timeRecord = TimeRecord.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            nsr: nsr,                         // ← NSR
            type: RecordType.ClockOut,
            //authenticationType: authType,
            signatureHash: hash,              // ← Hash
            latitude: request.Latitude,
            longitude: request.Longitude,
            ipAddress: request.IpAddress,     // ← Opcional
            userAgent: request.UserAgent,     // ← Opcional
            deviceId: deviceId,               // ← DeviceId
            notes: request.Notes
        );

        await timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Clock-out registered: {RecordId} (NSR: {NSR}) for user {UserId}", 
            timeRecord.Id, nsr, request.UserId);

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