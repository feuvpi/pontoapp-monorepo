using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetUserRecordsQueryHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository)
    : IRequestHandler<GetUserRecordsQuery, IEnumerable<TimeRecordResponse>>
{
    public async Task<IEnumerable<TimeRecordResponse>> Handle(GetUserRecordsQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado.");
        }

        var startDate = request.StartDate ?? DateTime.UtcNow.Date.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.UtcNow.Date.AddDays(1);

        var records = await timeRecordRepository.GetByUserIdAndDateRangeAsync(
            request.UserId,
            startDate,
            endDate,
            cancellationToken
        );

        return records.Select(r => new TimeRecordResponse
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
        });
    }
}