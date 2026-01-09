using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

public class UpdateTimeRecordCommand : IRequest<TimeRecordResponse>
{
    public Guid RecordId { get; set; }
    public string? RecordType { get; set; }
    public DateTime? RecordedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}