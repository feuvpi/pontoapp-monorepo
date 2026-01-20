using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Identity;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Handler para aprovação de ajuste de ponto
/// Cria novo TimeRecord com IsAdjustment=true e atualiza status do adjustment
/// </summary>
public class ApproveAdjustmentCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    ITimeRecordAdjustmentRepository adjustmentRepository,
    IUserRepository userRepository,
    ITenantAccessor tenantAccessor,
    IClaimsService claimsService,
    INSRGenerator nsrGenerator,
    ISignatureGenerator signatureGenerator,
    ILogger<ApproveAdjustmentCommandHandler> logger)
    : IRequestHandler<ApproveAdjustmentCommand, AdjustmentResponse>
{
    public async Task<AdjustmentResponse> Handle(
        ApproveAdjustmentCommand request,
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
            throw new BusinessException("Você não tem permissão para aprovar ajustes");
        }

        logger.LogInformation(
            "Approving adjustment {AdjustmentId} by user {UserId}",
            request.AdjustmentId, currentUserId);

        // 1. Buscar ajuste
        var adjustment = await adjustmentRepository.GetByIdWithDetailsAsync(
            request.AdjustmentId,
            cancellationToken);

        if (adjustment == null)
            throw new NotFoundException("Ajuste não encontrado");

        if (adjustment.Status != AdjustmentStatus.Pending)
            throw new BusinessException($"Ajuste já foi {adjustment.Status}");

        // 2. Buscar registro original
        var originalRecord = await timeRecordRepository.GetByIdAsync(
            adjustment.OriginalRecordId,
            cancellationToken);

        if (originalRecord == null)
            throw new NotFoundException("Registro original não encontrado");

        // 3. Buscar usuário (para CPF)
        var user = await userRepository.GetByIdAsync(originalRecord.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado");

        // 4. Não pode aprovar seu próprio ajuste
        if (adjustment.RequestedBy == currentUserId.Value)
            throw new BusinessException("Você não pode aprovar seu próprio ajuste");

        // 5. Gerar NSR
        var nsr = await nsrGenerator.GenerateNextAsync(tenantInfo.TenantId, cancellationToken);

        // 6. Determinar tipo (usa o novo se fornecido, senão mantém original)
        var recordType = adjustment.NewType ?? originalRecord.Type;

        // 7. Gerar hash
        var hash = signatureGenerator.GenerateHash(
            nsr: nsr,
            tenantId: tenantInfo.TenantId,
            userId: user.Id,
            cpf: user.CPF.Value,
            recordedAt: adjustment.NewRecordedAt,
            recordType: recordType.ToString());

        // 8. Criar novo TimeRecord (ajuste)
        var adjustmentRecord = TimeRecord.CreateAdjustment(
            tenantId: tenantInfo.TenantId,
            userId: user.Id,
            nsr: nsr,
            type: recordType,
            recordedAt: adjustment.NewRecordedAt,
            authenticationType: originalRecord.AuthenticationType,
            signatureHash: hash,
            originalTimeRecordId: originalRecord.Id,
            latitude: originalRecord.Latitude,
            longitude: originalRecord.Longitude,
            ipAddress: originalRecord.IpAddress,
            userAgent: originalRecord.UserAgent,
            notes: $"Ajuste aprovado. Motivo: {adjustment.Reason}");

        await timeRecordRepository.AddAsync(adjustmentRecord, cancellationToken);

        // 9. Atualizar status do ajuste
        adjustment.Approve(currentUserId.Value, adjustmentRecord.Id);

        adjustmentRepository.Update(adjustment);
        await adjustmentRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Adjustment approved: {AdjustmentId}, new record created: {RecordId}",
            adjustment.Id, adjustmentRecord.Id);

        // TODO: Notificar solicitante (futuro)

        return new AdjustmentResponse
        {
            Id = adjustment.Id,
            OriginalRecordId = originalRecord.Id,
            OriginalRecordedAt = originalRecord.RecordedAt,
            NewRecordedAt = adjustment.NewRecordedAt,
            NewRecordId = adjustmentRecord.Id,
            Reason = adjustment.Reason,
            Status = adjustment.Status.ToString(),
            RequestedBy = adjustment.RequestedBy,
            RequestedAt = adjustment.RequestedAt,
            ApprovedBy = currentUserId.Value,
            ApprovedAt = adjustment.ApprovedAt
        };
    }
}