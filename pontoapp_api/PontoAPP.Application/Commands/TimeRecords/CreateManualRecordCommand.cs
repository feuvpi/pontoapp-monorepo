using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

public class CreateManualRecordCommand : IRequest<TimeRecordResponse>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string RecordType { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}