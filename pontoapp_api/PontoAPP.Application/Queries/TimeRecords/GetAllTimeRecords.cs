using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Queries.TimeRecords;

public class GetAllTimeRecordsQuery : IRequest<IEnumerable<TimeRecordResponse>>
{
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? RecordType { get; set; }
}