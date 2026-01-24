using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Application.Commands.TimeRecords;

public class CreateManualRecordCommand : IRequest<TimeRecordResponse>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public RecordType RecordType { get; set; } 
    public DateTime RecordedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}