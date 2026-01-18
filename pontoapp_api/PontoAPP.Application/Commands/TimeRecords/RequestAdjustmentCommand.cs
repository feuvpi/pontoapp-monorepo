using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Command para solicitar ajuste de registro de ponto
/// Funcionário solicita → Gestor/RH aprova ou rejeita
/// </summary>
public class RequestAdjustmentCommand : IRequest<AdjustmentResponse>
{
    public Guid OriginalRecordId { get; set; }
    public DateTime NewRecordedAt { get; set; }
    public string Reason { get; set; } = string.Empty;
    
    // Opcional: se quer mudar o tipo também
    public string? NewType { get; set; }
}