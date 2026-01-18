namespace PontoAPP.Application.DTOs.TimeRecords;

/// <summary>
/// Response com dados do registro de ponto
/// ATUALIZADO: Inclui NSR e SignatureHash (Portaria 671)
/// </summary>
public class TimeRecordResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    
    // NOVO: Portaria 671
    public long NSR { get; set; }                    // Número Sequencial de Registro
    public string SignatureHash { get; set; } = string.Empty; // Hash SHA-256
    
    public DateTime RecordedAt { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AuthenticationType { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Opcional: se for ajuste
    public bool IsAdjustment { get; set; }
    public Guid? OriginalRecordId { get; set; }
}