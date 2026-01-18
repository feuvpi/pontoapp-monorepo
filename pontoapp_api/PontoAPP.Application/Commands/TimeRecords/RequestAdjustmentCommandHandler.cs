using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Handler para solicitação de ajuste de ponto
/// Cria registro em time_record_adjustments com status Pending
/// </summary>
public class RequestAdjustmentCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    ITimeRecordAdjustmentRepository adjustmentRepository,
    ITenantAccessor tenantAccessor,
    IClaimsService claimsService,
    ILogger<RequestAdjustmentCommandHandler> logger)
    : IRequestHandler<RequestAdjustmentCommand, AdjustmentResponse>
{
    public async Task<AdjustmentResponse> Handle(
        RequestAdjustmentCommand request,
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            throw new BusinessException("Tenant não identificado");

        var currentUserId = claimsService.GetCurrentUserId();
        if (currentUserId == null)
            throw new BusinessException("Usuário não identificado");

        logger.LogInformation(
            "Requesting adjustment for record {RecordId} by user {UserId}",
            request.OriginalRecordId, currentUserId);

        // 1. Buscar registro original
        var originalRecord = await timeRecordRepository.GetByIdAsync(
            request.OriginalRecordId, 
            cancellationToken);

        if (originalRecord == null)
            throw new NotFoundException("Registro de ponto não encontrado");

        // 2. Validar que é do mesmo usuário
        if (originalRecord.UserId != currentUserId.Value)
            throw new BusinessException("Você só pode solicitar ajuste dos seus próprios registros");

        // 3. Validar que não é ajuste de ajuste
        // if (originalRecord.IsAdjustment)
        //     throw new BusinessException("Não é possível ajustar um registro que já é um ajuste");

        // 4. Validar que não tem ajuste pendente
        var existingAdjustment = await adjustmentRepository
            .GetPendingAdjustmentByRecordIdAsync(request.OriginalRecordId, cancellationToken);

        if (existingAdjustment != null)
            throw new BusinessException("Já existe um ajuste pendente para este registro");

        // 5. Parse do novo tipo (se fornecido)
        RecordType? newType = null;
        if (!string.IsNullOrWhiteSpace(request.NewType))
        {
            if (!Enum.TryParse<RecordType>(request.NewType, true, out var parsedType))
                throw new ValidationException("Tipo de registro inválido");
            
            newType = parsedType;
        }

        // 6. Criar solicitação de ajuste
        var adjustment = TimeRecordAdjustment.Create(
            tenantId: tenantInfo.TenantId,
            originalRecordId: originalRecord.Id,
            originalRecordedAt: originalRecord.RecordedAt,
            newRecordedAt: request.NewRecordedAt,
            newType: newType,
            reason: request.Reason,
            requestedBy: currentUserId.Value);

        await adjustmentRepository.AddAsync(adjustment, cancellationToken);
        await adjustmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Adjustment requested: {AdjustmentId} for record {RecordId}",
            adjustment.Id, originalRecord.Id);

        // TODO: Enviar notificação para gestores/RH (futuro)

        return new AdjustmentResponse
        {
            Id = adjustment.Id,
            OriginalRecordId = originalRecord.Id,
            OriginalRecordedAt = originalRecord.RecordedAt,
            NewRecordedAt = request.NewRecordedAt,
            Reason = request.Reason,
            Status = adjustment.Status.ToString(),
            RequestedBy = currentUserId.Value,
            RequestedAt = adjustment.RequestedAt
        };
    }
}