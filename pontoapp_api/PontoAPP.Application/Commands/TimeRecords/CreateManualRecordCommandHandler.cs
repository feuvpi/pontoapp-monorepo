using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.TimeRecords;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.Application.Commands.TimeRecords;

/// <summary>
/// Handler para criar registro manual de ponto (Admin/HR/Manager)
/// Usado quando precisa lançar ponto retroativo ou corrigir
/// </summary>
public class CreateManualRecordCommandHandler(
    ITimeRecordRepository timeRecordRepository,
    IUserRepository userRepository,
    ITenantAccessor tenantAccessor,
    INSRGenerator nsrGenerator,
    ISignatureGenerator signatureGenerator,
    ILogger<CreateManualRecordCommandHandler> logger)
    : IRequestHandler<CreateManualRecordCommand, TimeRecordResponse>
{
    public async Task<TimeRecordResponse> Handle(
        CreateManualRecordCommand request, 
        CancellationToken cancellationToken)
    {
        var tenantInfo = tenantAccessor.GetTenantInfo();
        if (tenantInfo == null)
            throw new BusinessException("Tenant não identificado");

        logger.LogInformation(
            "Creating manual record for user {UserId} in tenant {TenantId}",
            request.UserId, tenantInfo.TenantId);

        // 1. Buscar usuário (para pegar CPF)
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            throw new NotFoundException("Usuário não encontrado");

        if (!user.IsActive)
            throw new BusinessException("Usuário inativo");

        // ========== PORTARIA 671: NSR + HASH ==========
        
        // 2. Gerar NSR (atômico e thread-safe)
        var nsr = await nsrGenerator.GenerateNextAsync(tenantInfo.TenantId, cancellationToken);
        logger.LogInformation("Generated NSR: {NSR}", nsr);

        // 3. Gerar hash de assinatura
        var hash = signatureGenerator.GenerateHash(
            nsr: nsr,
            tenantId: tenantInfo.TenantId,
            userId: user.Id,
            cpf: user.CPF.Value,
            recordedAt: request.RecordedAt,
            recordType: request.RecordType.ToString());
        
        logger.LogDebug("Generated signature hash for NSR {NSR}", nsr);

        // ========== FIM PORTARIA 671 ==========

        // 4. Criar TimeRecord com NSR e Hash
        var timeRecord = TimeRecord.Create(
            tenantId: tenantInfo.TenantId,
            userId: user.Id,
            nsr: nsr,                         // ← NSR gerado
            type: request.RecordType,
            //authenticationType: null, 
            signatureHash: hash,              // ← Hash gerado
            latitude: request.Latitude,
            longitude: request.Longitude,
            deviceId: null,                   // Manual não tem device
            notes: request.Notes);

        // Se for manual, ajustar RecordedAt para a data fornecida
        // NOTA: TimeRecord.Create() usa DateTime.UtcNow, mas para manual queremos a data fornecida
        // Como TimeRecord é imutável, precisamos usar reflexão OU criar método específico
        // Por enquanto, vamos criar um método CreateManual() no TimeRecord
        
        // TODO: Adicionar TimeRecord.CreateManual() que aceita recordedAt customizado
        // Por enquanto, o TimeRecord vai usar DateTime.UtcNow

        // 5. Salvar
        await timeRecordRepository.AddAsync(timeRecord, cancellationToken);
        await timeRecordRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Manual record created: {RecordId} with NSR {NSR}",
            timeRecord.Id, nsr);

        return new TimeRecordResponse
        {
            Id = timeRecord.Id,
            UserId = user.Id,
            UserName = user.FullName,
            NSR = nsr,
            RecordedAt = timeRecord.RecordedAt,
            Type = timeRecord.Type.ToString(),
            Status = timeRecord.Status.ToString(),
            AuthenticationType = timeRecord.AuthenticationType.ToString(),
            SignatureHash = hash,
            Latitude = timeRecord.Latitude,
            Longitude = timeRecord.Longitude,
            Notes = timeRecord.Notes,
            CreatedAt = timeRecord.CreatedAt
        };
    }
}