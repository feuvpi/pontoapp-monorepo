using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PontoAPP.Domain.Services;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Services;

/// <summary>
/// Implementação de INSRGenerator usando stored procedure PostgreSQL
/// Garante geração atômica e thread-safe do NSR por tenant
/// </summary>
public class NSRGenerator : INSRGenerator
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<NSRGenerator> _logger;

    public NSRGenerator(AppDbContext dbContext, ILogger<NSRGenerator> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<long> GenerateNextAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating NSR for tenant: {TenantId}", tenantId);

            // Chama stored procedure que faz UPDATE atomicamente
            // A stored procedure garante:
            // 1. Lock pessimista (FOR UPDATE)
            // 2. Incremento atômico
            // 3. Cria contador se não existe
            var nsr = await _dbContext.Database
                .SqlQueryRaw<long>("SELECT get_next_nsr({0})", tenantId)
                .FirstAsync(cancellationToken);

            _logger.LogInformation("Generated NSR {NSR} for tenant {TenantId}", nsr, tenantId);

            return nsr;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate NSR for tenant {TenantId}", tenantId);
            throw new InvalidOperationException($"Failed to generate NSR for tenant {tenantId}", ex);
        }
    }
}