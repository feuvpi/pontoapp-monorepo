using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetDailySummaryQueryHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetDailySummaryQuery, DailySummaryResponse>
{
    public async Task<DailySummaryResponse> Handle(GetDailySummaryQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        var date = request.Date?.Date ?? DateTime.UtcNow.Date;
        var startOfDay = date;
        var endOfDay = date.AddDays(1);

        var records = await timeRecordRepository.GetByUserIdAndDateRangeAsync(
            request.UserId,
            startOfDay,
            endOfDay,
            cancellationToken
        );

        var recordsList = records
            .Where(r => r.Status != RecordStatus.Rejected)
            .OrderBy(r => r.RecordedAt)
            .ToList();

        // Calcular horas trabalhadas
        var totalWorked = CalculateWorkedTime(recordsList);

        // Determinar status atual
        var lastRecord = recordsList.LastOrDefault();
        var currentStatus = DetermineCurrentStatus(lastRecord);

        // Verificar se tem pares completos
        var isComplete = recordsList.Count % 2 == 0 && recordsList.Count > 0;

        return new DailySummaryResponse
        {
            Date = date,
            UserId = request.UserId,
            UserName = user.FullName,
            Records = recordsList.Select(r => new TimeRecordResponse
            {
                Id = r.Id,
                UserId = r.UserId,
                UserName = user.FullName,
                RecordedAt = r.RecordedAt,
                Type = r.Type.ToString(),
                Status = r.Status.ToString(),
                AuthenticationType = r.AuthenticationType.ToString(),
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            }).ToList(),
            TotalWorkedTime = totalWorked,
            TotalRecords = recordsList.Count,
            IsComplete = isComplete,
            CurrentStatus = currentStatus
        };
    }

    private static TimeSpan CalculateWorkedTime(List<Domain.Entities.TimeTracking.TimeRecord> records)
    {
        var totalWorked = TimeSpan.Zero;
        
        for (int i = 0; i < records.Count - 1; i += 2)
        {
            var clockIn = records[i];
            var clockOut = records.ElementAtOrDefault(i + 1);

            if (clockIn.Type == RecordType.ClockIn && clockOut?.Type == RecordType.ClockOut)
            {
                totalWorked += clockOut.RecordedAt - clockIn.RecordedAt;
            }
        }

        // Se último registro é entrada (ainda trabalhando), calcular até agora
        var lastRecord = records.LastOrDefault();
        if (lastRecord?.Type == RecordType.ClockIn)
        {
            totalWorked += DateTime.UtcNow - lastRecord.RecordedAt;
        }

        return totalWorked;
    }

    private static string DetermineCurrentStatus(Domain.Entities.TimeTracking.TimeRecord? lastRecord)
    {
        if (lastRecord == null)
            return "Sem registros";

        return lastRecord.Type == RecordType.ClockIn ? "Trabalhando" : "Fora";
    }
}