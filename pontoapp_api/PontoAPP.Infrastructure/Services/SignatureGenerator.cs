using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Services;

namespace PontoAPP.Infrastructure.Services;

/// <summary>
/// Implementação de ISignatureGenerator usando SHA-256
/// Gera hash para garantir integridade dos registros de ponto (Portaria 671)
/// </summary>
public class SignatureGenerator : ISignatureGenerator
{
    private readonly ILogger<SignatureGenerator> _logger;

    public SignatureGenerator(ILogger<SignatureGenerator> logger)
    {
        _logger = logger;
    }

    public string GenerateHash(
        long nsr,
        Guid tenantId,
        Guid userId,
        string cpf,
        DateTime recordedAt,
        string recordType)
    {
        try
        {
            // Formato: NSR|TenantId|UserId|CPF|RecordedAt|Type
            // Exemplo: 1543|aaa-111-222|bbb-333-444|12345678901|20260116143000|ClockIn
            var data = $"{nsr}|{tenantId}|{userId}|{cpf}|{recordedAt:yyyyMMddHHmmss}|{recordType}";

            _logger.LogDebug("Generating hash for data: {Data}", data);

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(data);
            var hashBytes = sha256.ComputeHash(bytes);
            
            // Converte para hexadecimal
            var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            _logger.LogDebug("Generated hash: {Hash}", hash);

            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate hash");
            throw new InvalidOperationException("Failed to generate signature hash", ex);
        }
    }

    public bool ValidateHash(TimeRecord record, string userCpf)
    {
        try
        {
            // Regenera o hash com os dados originais
            var expectedHash = GenerateHash(
                record.NSR,
                record.TenantId,
                record.UserId,
                userCpf,
                record.RecordedAt,
                record.Type.ToString());

            // Compara com o hash armazenado
            var isValid = string.Equals(
                expectedHash, 
                record.SignatureHash, 
                StringComparison.OrdinalIgnoreCase);

            if (!isValid)
            {
                _logger.LogWarning(
                    "Hash validation failed for TimeRecord {RecordId}. Expected: {Expected}, Got: {Actual}",
                    record.Id, expectedHash, record.SignatureHash);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate hash for TimeRecord {RecordId}", record.Id);
            return false;
        }
    }
}