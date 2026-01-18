using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Handler para rejeição de ajuste de ponto
/// Atualiza status para Rejected e adiciona motivo
/// </summary>
public class RejectAdjustmentCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    ITimeRecordAdjustmentRepository adjustmentRepository,
    ITenantAccessor tenantAccessor,
    IClaimsService claimsService,
    ILogger<RejectAdjustmentCommandHandler> logger)
    : IRequestHandler<RejectAdjustmentCommand, AdjustmentResponse>
{
    public async Task<AdjustmentResponse> Handle(
        RejectAdjustmentCommand request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            throw new BusinessException("Tenant não identificado");

        var currentUserId = claimsService.GetCurrentUserId();
        if (currentUserId == null)
            throw new BusinessException("Usuário não identificado");

        // Verificar permissão (apenas Admin/HR/Manager)
        if (!claimsService.IsAdmin() && !claimsService.IsManager() && 
            !claimsService.IsInRole("HR"))
        {
            throw new BusinessException("Você não tem permissão para rejeitar ajustes");
        }

        logger.LogInformation(
            "Rejecting adjustment {AdjustmentId} by user {UserId}",
            request.AdjustmentId, currentUserId);

        // 1. Buscar ajuste
        var adjustment = await adjustmentRepository.GetByIdWithDetailsAsync(
            request.AdjustmentId,
            cancellationToken);

        if (adjustment == null)
            throw new NotFoundException("Ajuste não encontrado");

        if (adjustment.Status != AdjustmentStatus.Pending)
            throw new BusinessException($"Ajuste já foi {adjustment.Status}");

        // 2. Buscar registro original (para response)
        var originalRecord = await timeRecordRepository.GetByIdAsync(
            adjustment.OriginalRecordId,
            cancellationToken);

        if (originalRecord == null)
            throw new NotFoundException("Registro original não encontrado");

        // 3. Não pode rejeitar seu próprio ajuste
        if (adjustment.RequestedBy == currentUserId.Value)
            throw new BusinessException("Você não pode rejeitar seu próprio ajuste");

        // 4. Rejeitar
        adjustment.Reject(currentUserId.Value, request.RejectionReason);

        adjustmentRepository.Update(adjustment);
        await adjustmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Adjustment rejected: {AdjustmentId}, reason: {Reason}",
            adjustment.Id, request.RejectionReason);

        // TODO: Notificar solicitante (futuro)

        return new AdjustmentResponse
        {
            Id = adjustment.Id,
            OriginalRecordId = originalRecord.Id,
            OriginalRecordedAt = originalRecord.RecordedAt,
            NewRecordedAt = adjustment.NewRecordedAt,
            Reason = adjustment.Reason,
            RejectionReason = adjustment.RejectionReason,
            Status = adjustment.Status.ToString(),
            RequestedBy = adjustment.RequestedBy,
            RequestedAt = adjustment.RequestedAt,
            ApprovedBy = currentUserId.Value,
            ApprovedAt = adjustment.ApprovedAt
        };
    }
}