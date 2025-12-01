namespace PontoAPP.Application.DTOs.TimeRecords;

public class ClockOutRequest
{
    public string DeviceId { get; set; } = string.Empty; // NOVO: obrigatório
    public string AuthenticationType { get; set; } = "Password";
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
}