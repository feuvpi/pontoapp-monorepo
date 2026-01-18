namespace PontoAPP.Application.DTOs.TimeRecords;

/// <summary>
/// Request para solicitar ajuste de ponto
/// </summary>
public class RequestAdjustmentRequest
{
    public Guid OriginalRecordId { get; set; }
    public DateTime NewRecordedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    
    /// <summary>
    /// Opcional: se quer mudar o tipo também
    /// Ex: "ClockIn", "ClockOut", "BreakStart", "BreakEnd"
    /// </summary>
    public string? NewType { get; set; }
}