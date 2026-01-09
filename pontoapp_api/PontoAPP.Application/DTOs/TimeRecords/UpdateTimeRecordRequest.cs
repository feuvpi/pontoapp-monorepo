namespace PontoAPP.Application.DTOs.TimeRecords;

public class UpdateTimeRecordRequest
{
    public string? RecordType { get; set; }
    public DateTime? RecordedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}