using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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

            // Usa ExecuteSqlRaw para chamar stored procedure
            // Retorna o valor diretamente sem mapear para entity
            var connection = _dbContext.Database.GetDbConnection();
            await using var command = connection.CreateCommand();
            
            command.CommandText = "SELECT get_next_nsr(@p_tenant_id)";
            command.Parameters.Add(new NpgsqlParameter("@p_tenant_id", tenantId));

            // Garante que a conexão está aberta
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            // Executa e obtém o resultado
            var result = await command.ExecuteScalarAsync(cancellationToken);
            var nsr = Convert.ToInt64(result);

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