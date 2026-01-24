namespace PontoAPP.Domain.Services;

/// <summary>
/// Serviço para geração do Espelho de Ponto mensal em PDF
/// </summary>
public interface IEspelhoPontoGenerator
{
    /// <summary>
    /// Gera o Espelho de Ponto em formato PDF
    /// </summary>
    /// <param name="tenantId">ID do tenant (empresa)</param>
    /// <param name="userId">ID do funcionário</param>
    /// <param name="year">Ano</param>
    /// <param name="month">Mês (1-12)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Byte array do PDF gerado</returns>
    Task<byte[]> GeneratePDFAsync(
        Guid tenantId,
        Guid userId,
        int year,
        int month,
        CancellationToken cancellationToken = default);
}