using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.TimeRecords;

public class ClockOutCommandHandler : IRequestHandler<ClockOutCommand, TimeRecordResponse>
{
    private readonly ITimeRecordRepository _timeRecordRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ClockOutCommandHandler> _logger;

    private const int MinimumIntervalMinutes = 1;

    public ClockOutCommandHandler(
        ITimeRecordRepository timeRecordRepository,
        IUserRepository userRepository,
        ILogger<ClockOutCommandHandler> logger)
    {
        _timeRecordRepository = timeRecordRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<TimeRecordResponse> Handle(ClockOutCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Clock-out attempt for user: {UserId}", request.UserId);

        // Validar usuário
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        if (!user.IsActive)
        {
            throw new BusinessException("Usuário inativo. Não é possível registrar ponto.");
        }

        // Validar último registro
        var lastRecord = await _timeRecordRepository.GetLastRecordByUserIdAsync(request.UserId, cancellationToken);

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

        // Criar registro
        var timeRecord = TimeRecord.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            type: RecordType.ClockOut,
            authenticationType: authType,
            latitude: request.Latitude,
            longitude: request.Longitude,
            notes: request.Notes
        );

        await _timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await _timeRecordRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Clock-out registered: {RecordId} for user {UserId}", timeRecord.Id, request.UserId);

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