namespace PontoAPP.Application.DTOs.TimeRecords;
public class CreateManualRecordRequest
{
    public Guid UserId { get; set; }
    public string RecordType { get; set; } = string.Empty; // "ClockIn", "ClockOut", "BreakStart", "BreakEnd"
    public DateTime RecordedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}