namespace PontoAPP.Domain.Services;

public interface IAFDGenerator
{
    Task<string> GenerateAFDAsync(
        Guid tenantId,
        DateTime startDate,
        DateTime endDate,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}