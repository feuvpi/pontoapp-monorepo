using PontoAPP.Domain.Enums;

namespace PontoAPP.Application.DTOs.TimeRecords;

/// <summary>
/// Request para criar registro manual de ponto via API
/// </summary>
public class CreateManualRecordRequest
{
    public Guid UserId { get; set; }
    public DateTime RecordedAt { get; set; }
    public RecordType RecordType { get; set; } // "ClockIn", "ClockOut", etc
    public string AuthenticationType { get; set; } = "Manual";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}