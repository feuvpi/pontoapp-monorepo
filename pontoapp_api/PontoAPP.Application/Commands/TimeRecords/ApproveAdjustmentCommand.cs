using MediatR;
using PontoAPP.Application.DTOs.TimeRecords;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Command para aprovar ajuste de ponto (apenas Admin/HR/Manager)
/// Ao aprovar, cria novo TimeRecord com IsAdjustment=true
/// </summary>
public class ApproveAdjustmentCommand : IRequest<AdjustmentResponse>
{
    public Guid AdjustmentId { get; set; }
}