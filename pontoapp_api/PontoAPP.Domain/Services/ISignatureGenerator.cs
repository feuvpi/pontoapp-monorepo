using PontoAPP.Domain.Entities.TimeTracking;

namespace PontoAPP.Domain.Services;

/// <summary>
/// Domain Service para gerar e validar assinaturas digitais (hash) de registros de ponto
/// Garante integridade dos dados conforme Portaria 671
/// </summary>
public interface ISignatureGenerator
{
    /// <summary>
    /// Gera hash SHA-256 para um registro de ponto
    /// Hash baseado em: NSR, TenantId, UserId, CPF, RecordedAt, Type
    /// </summary>
    /// <param name="nsr">Número Sequencial do Registro</param>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="userId">ID do usuário</param>
    /// <param name="cpf">CPF do usuário</param>
    /// <param name="recordedAt">Data/hora do registro</param>
    /// <param name="recordType">Tipo do registro (entrada/saída)</param>
    /// <returns>Hash SHA-256 em formato hexadecimal</returns>
    string GenerateHash(
        long nsr,
        Guid tenantId,
        Guid userId,
        string cpf,
        DateTime recordedAt,
        string recordType);

    /// <summary>
    /// Valida se o hash de um TimeRecord é válido
    /// </summary>
    /// <param name="record">Registro a validar</param>
    /// <param name="userCpf">CPF do usuário (precisa buscar da User entity)</param>
    /// <returns>True se o hash está correto</returns>
    bool ValidateHash(TimeRecord record, string userCpf);
}