namespace PontoAPP.Application.DTOs.TimeRecords;

public class TimeRecordResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime RecordedAt { get; set; }
    public string Type { get; set; } = string.Empty; // ClockIn, ClockOut
    public string Status { get; set; } = string.Empty;
    public string AuthenticationType { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}