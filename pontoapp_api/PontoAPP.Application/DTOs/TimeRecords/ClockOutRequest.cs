namespace PontoAPP.Application.DTOs.TimeRecords;

/// <summary>
/// Request para registrar saída via API
/// </summary>
public class ClockOutRequest
{
    public string? DeviceId { get; set; }
    public string AuthenticationType { get; set; } = "Password";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // NOVO: Portaria 671 (mobile envia)
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public string? Notes { get; set; }
}