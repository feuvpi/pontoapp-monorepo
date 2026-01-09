using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.TimeRecords;

public class CreateManualRecordCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    ILogger<CreateManualRecordCommandHandler> logger)
    : IRequestHandler<CreateManualRecordCommand, TimeRecordResponse>
{
    public async Task<TimeRecordResponse> Handle(
        CreateManualRecordCommand request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating manual record for user: {UserId}, Type: {RecordType}, Time: {RecordedAt}", 
            request.UserId, request.RecordType, request.RecordedAt);

        // Validate user
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        // Validate record type
        if (!Enum.TryParse<RecordType>(request.RecordType, true, out var recordType))
        {
            throw new ValidationException($"Tipo de registro inválido: {request.RecordType}");
        }

        // Validate recorded time is not in the future
        if (request.RecordedAt > DateTime.UtcNow)
        {
            throw new ValidationException("Não é possível criar registro no futuro.");
        }

        // Create manual record (using Password as auth type for manual entries)
        var timeRecord = TimeRecord.Create(
            tenantId: request.TenantId,
            userId: request.UserId,
            type: recordType,
            authenticationType: AuthenticationType.Password, // Manual records
            latitude: request.Latitude,
            longitude: request.Longitude,
            notes: $"[MANUAL] {request.Notes ?? "Registro criado manualmente pelo administrador"}"
        );

        // Override recorded time (since it's manual)
        // You may need to add a method to TimeRecord entity for this
        // For now, using reflection (not ideal, but works)
        typeof(TimeRecord)
            .GetProperty(nameof(TimeRecord.RecordedAt))
            ?.SetValue(timeRecord, request.RecordedAt);

        await timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Manual record created: {RecordId} for user {UserId}", 
            timeRecord.Id, request.UserId);

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