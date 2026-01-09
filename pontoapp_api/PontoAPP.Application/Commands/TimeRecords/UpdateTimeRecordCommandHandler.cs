using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.TimeRecords;

public class UpdateTimeRecordCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    ILogger<UpdateTimeRecordCommandHandler> logger)
    : IRequestHandler<UpdateTimeRecordCommand, TimeRecordResponse>
{
    public async Task<TimeRecordResponse> Handle(
        UpdateTimeRecordCommand request, 
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating time record: {RecordId}", request.RecordId);

        var timeRecord = await timeRecordRepository.GetByIdAsync(request.RecordId, cancellationToken);
        if (timeRecord == null)
        {
            throw new NotFoundException("Registro não encontrado.");
        }

        // Update record type if provided
        if (!string.IsNullOrWhiteSpace(request.RecordType))
        {
            if (!Enum.TryParse<RecordType>(request.RecordType, true, out var recordType))
            {
                throw new ValidationException($"Tipo de registro inválido: {request.RecordType}");
            }
            
            // You may need to add UpdateType method to TimeRecord entity
            typeof(TimeRecord)
                .GetProperty(nameof(TimeRecord.Type))
                ?.SetValue(timeRecord, recordType);
        }

        // Update recorded time if provided
        if (request.RecordedAt.HasValue)
        {
            if (request.RecordedAt.Value > DateTime.UtcNow)
            {
                throw new ValidationException("Não é possível definir horário no futuro.");
            }

            typeof(TimeRecord)
                .GetProperty(nameof(TimeRecord.RecordedAt))
                ?.SetValue(timeRecord, request.RecordedAt.Value);
        }

        // Update location if provided
        if (request.Latitude.HasValue || request.Longitude.HasValue)
        {
            typeof(TimeRecord)
                .GetProperty(nameof(TimeRecord.Latitude))
                ?.SetValue(timeRecord, request.Latitude);
            
            typeof(TimeRecord)
                .GetProperty(nameof(TimeRecord.Longitude))
                ?.SetValue(timeRecord, request.Longitude);
        }

        // Update notes if provided
        if (request.Notes != null)
        {
            var updatedNotes = $"[EDITADO] {request.Notes}";
            typeof(TimeRecord)
                .GetProperty(nameof(TimeRecord.Notes))
                ?.SetValue(timeRecord, updatedNotes);
        }

        timeRecordRepository.Update(timeRecord);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        // Fetch user for response
        var user = await userRepository.GetByIdAsync(timeRecord.UserId, cancellationToken);

        logger.LogInformation("Time record updated: {RecordId}", request.RecordId);

        return new TimeRecordResponse
        {
            Id = timeRecord.Id,
            UserId = timeRecord.UserId,
            UserName = user?.FullName ?? "Unknown",
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
