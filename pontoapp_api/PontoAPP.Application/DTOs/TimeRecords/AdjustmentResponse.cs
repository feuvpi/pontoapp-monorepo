namespace PontoAPP.Application.DTOs.TimeRecords;

/// <summary>
/// Response com dados do ajuste de ponto
/// </summary>
public class AdjustmentResponse
{
    public Guid Id { get; set; }
    public Guid OriginalRecordId { get; set; }
    public DateTime OriginalRecordedAt { get; set; }
    public DateTime NewRecordedAt { get; set; }
    public Guid? NewRecordId { get; set; } // Preenchido após aprovação
    public string Reason { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAt { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
}