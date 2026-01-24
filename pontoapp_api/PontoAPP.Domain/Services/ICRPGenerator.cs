namespace PontoAPP.Domain.Services;

/// <summary>
/// Serviço para geração do CRP (Comprovante de Registro de Ponto) em PDF
/// </summary>
public interface ICRPGenerator
{
    /// <summary>
    /// Gera o Comprovante de Registro de Ponto em formato PDF
    /// </summary>
    /// <param name="timeRecordId">ID do registro de ponto</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Byte array do PDF gerado</returns>
    Task<byte[]> GeneratePDFAsync(
        Guid timeRecordId,
        CancellationToken cancellationToken = default);
}