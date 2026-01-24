using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

public class ClockOutCommand : IRequest<TimeRecordResponse>
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string AuthenticationType { get; set; } = "Password";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceId { get; set; } // ← Adicionar (estava faltando)
}