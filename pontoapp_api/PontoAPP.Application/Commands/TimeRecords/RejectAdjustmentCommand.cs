using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Command para rejeitar ajuste de ponto (apenas Admin/HR/Manager)
/// </summary>
public class RejectAdjustmentCommand : IRequest<AdjustmentResponse>
{
    public Guid AdjustmentId { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
}