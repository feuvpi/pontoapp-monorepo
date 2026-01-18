namespace PontoAPP.Domain.Services;

/// <summary>
/// Domain Service para gerar NSR (Número Sequencial de Registro)
/// NSR é único e crescente por tenant, conforme Portaria 671
/// </summary>
public interface INSRGenerator
{
    /// <summary>
    /// Gera o próximo NSR para um tenant
    /// Thread-safe e atômico (usa stored procedure com lock)
    /// </summary>
    /// <param name="tenantId">ID do tenant</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Próximo NSR sequencial</returns>
    Task<long> GenerateNextAsync(Guid tenantId, CancellationToken cancellationToken = default);
}