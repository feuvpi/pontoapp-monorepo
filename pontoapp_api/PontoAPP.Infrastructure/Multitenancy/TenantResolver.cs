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
public class TenantResolver(IServiceProvider serviceProvider, ILogger<TenantResolver> logger)
    : ITenantResolver
{
    public async Task<TenantInfo?> ResolveAsync(HttpContext httpContext)
{
    logger.LogInformation("========== TENANT RESOLVER START ==========");
    
    string? identifier = null;
    string strategy = "unknown";
    
    // Log do estado de autenticação
    logger.LogInformation("User IsAuthenticated: {IsAuth}", httpContext.User?.Identity?.IsAuthenticated);
    logger.LogInformation("User Identity Name: {Name}", httpContext.User?.Identity?.Name);
    
    // Estratégia 1: JWT Claim (usuário autenticado)
    if (httpContext.User.Identity?.IsAuthenticated == true)
    {
        // LOG TODOS OS CLAIMS
        var allClaims = httpContext.User.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
        logger.LogInformation("All JWT Claims: {Claims}", string.Join(" | ", allClaims));
        
        // Tentar diferentes variações do nome do claim
        var tenantClaim = httpContext.User.FindFirst("tenantId")?.Value;
        
        if (string.IsNullOrEmpty(tenantClaim))
        {
            logger.LogWarning("Claim 'tenant_id' not found, trying 'tenantId'");
            tenantClaim = httpContext.User.FindFirst("tenantId")?.Value;
        }
        
        if (string.IsNullOrEmpty(tenantClaim))
        {
            logger.LogWarning("Claim 'tenantId' not found, trying 'TenantId'");
            tenantClaim = httpContext.User.FindFirst("TenantId")?.Value;
        }
        
        if (string.IsNullOrEmpty(tenantClaim))
        {
            logger.LogWarning("Claim 'TenantId' not found, trying 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/tenant'");
            tenantClaim = httpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/tenant")?.Value;
        }
        
        if (!string.IsNullOrEmpty(tenantClaim))
        {
            identifier = tenantClaim;
            strategy = "jwt";
            logger.LogInformation("✅ Tenant ID found in JWT: {TenantId}", tenantClaim);
        }
        else
        {
            logger.LogError("❌ NO TENANT CLAIM FOUND IN JWT!");
        }
    }
    else
    {
        logger.LogWarning("User is NOT authenticated");
    }
    
    // Estratégia 2: Header X-Tenant-Id
    if (string.IsNullOrEmpty(identifier) && 
        httpContext.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeader))
    {
        identifier = tenantHeader.ToString();
        strategy = "header";
        logger.LogInformation("✅ Tenant ID found in Header: {TenantId}", identifier);
    }

    // Estratégia 3: Subdomain
    if (string.IsNullOrEmpty(identifier))
    {
        var host = httpContext.Request.Host.Host;
        logger.LogInformation("Host: {Host}", host);
        
        var parts = host.Split('.');
        
        if (parts.Length >= 3 && parts[0] != "www" && parts[0] != "api")
        {
            identifier = parts[0];
            strategy = "subdomain";
            logger.LogInformation("✅ Tenant ID found in Subdomain: {TenantId}", identifier);
        }
    }

    // Estratégia 4: Query String (para testes)
    if (string.IsNullOrEmpty(identifier) && httpContext.Request.Query.ContainsKey("tenantId"))
    {
        identifier = httpContext.Request.Query["tenantId"].ToString();
        strategy = "querystring";
        logger.LogInformation("✅ Tenant ID found in QueryString: {TenantId}", identifier);
    }

    if (string.IsNullOrEmpty(identifier))
    {
        logger.LogError("❌ NO TENANT IDENTIFIER FOUND - All strategies failed");
        logger.LogInformation("========== TENANT RESOLVER END (FAILED) ==========");
        return null;
    }

    logger.LogInformation("Tenant identifier: {Identifier} (strategy: {Strategy})", identifier, strategy);
    
    var result = await GetTenantInfoAsync(identifier, strategy);
    
    if (result != null)
    {
        logger.LogInformation("✅ Tenant resolved successfully: {TenantName} ({TenantId})", result.TenantName, result.TenantId);
    }
    else
    {
        logger.LogError("❌ GetTenantInfoAsync returned NULL");
    }
    
    logger.LogInformation("========== TENANT RESOLVER END ==========");
    return result;
    
}
    
    
    private async Task<TenantInfo?> GetTenantInfoAsync(string identifier, string strategy)
    {
        using var scope = serviceProvider.CreateScope();
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
            logger.LogWarning("Tenant not found: {Identifier} (strategy: {Strategy})", 
                identifier, strategy);
            return null;
        }

        logger.LogDebug("Tenant resolved: {TenantName} (strategy: {Strategy})", 
            tenant.Name, strategy);

        return new TenantInfo(tenant.Id, tenant.Name, tenant.Slug, tenant.IsActive);
    }
}
