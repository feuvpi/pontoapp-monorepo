using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Multitenancy;

/// <summary>
/// Resolve o tenant usando múltiplas estratégias:
/// 1. Header "X-Tenant-Id" (para APIs)
/// 2. Subdomain (ex: empresa.pontoapp.com)
/// 3. Query string "tenantId" (fallback)
/// </summary>
public class TenantResolver : ITenantResolver
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantResolver> _logger;

    public TenantResolver(IServiceProvider serviceProvider, ILogger<TenantResolver> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<TenantInfo?> ResolveAsync(HttpContext httpContext)
    {
        string? identifier = null;
        string strategy = "unknown";
        
        // Estratégia 1: JWT Claim (usuário autenticado)
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenantId")?.Value;
            if (!string.IsNullOrEmpty(tenantClaim))
            {
                identifier = tenantClaim;
                strategy = "jwt";
            }
        }
        
        // Estratégia 2: Header X-Tenant-Id
        if (string.IsNullOrEmpty(identifier) && 
            httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
        {
            identifier = tenantHeader.ToString();
            strategy = "header";
        }

        // Estratégia 3: Subdomain
        if (string.IsNullOrEmpty(identifier))
        {
            var host = httpContext.Request.Host.Host;
            var parts = host.Split('.');
            
            if (parts.Length >= 3 && parts[0] != "www" && parts[0] != "api")
            {
                identifier = parts[0];
                strategy = "subdomain";
            }
        }


        // Estratégia 3: Query String (para testes)
        if (string.IsNullOrEmpty(identifier) && httpContext.Request.Query.ContainsKey("tenantId"))
        {
            identifier = httpContext.Request.Query["tenantId"].ToString();
            strategy = "querystring";
        }

        if (string.IsNullOrEmpty(identifier))
        {
            _logger.LogWarning("No tenant identifier found in request");
            return null;
        }

        return await GetTenantInfoAsync(identifier, strategy);
    }
    
    
    
    
    private async Task<TenantInfo?> GetTenantInfoAsync(string identifier, string strategy)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tenant = Guid.TryParse(identifier, out var tenantId)
            ? await dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId)
            : await dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Slug == identifier.ToLower());

        if (tenant == null)
        {
            _logger.LogWarning("Tenant not found: {Identifier} (strategy: {Strategy})", 
                identifier, strategy);
            return null;
        }

        _logger.LogDebug("Tenant resolved: {TenantName} (strategy: {Strategy})", 
            tenant.Name, strategy);

        return new TenantInfo(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive);
    }
}
