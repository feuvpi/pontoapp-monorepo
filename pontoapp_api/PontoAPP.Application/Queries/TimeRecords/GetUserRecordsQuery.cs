using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetUserRecordsQuery : IRequest<IEnumerable<TimeRecordResponse>>
{
    public Guid UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}