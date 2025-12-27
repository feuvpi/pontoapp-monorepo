using Microsoft.EntityFrameworkCore;
using PontoAPP.Infrastructure.Data.Context;
using PontoAPP.Infrastructure.Multitenancy;

namespace PontoAPP.API.Middleware;

/// <summary>
/// Middleware que resolve o tenant da requisição e popula o TenantAccessor
/// </summary>
public class TenantResolutionMiddleware(
    RequestDelegate next,
    ILogger<TenantResolutionMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        ITenantResolver tenantResolver,
        ITenantAccessor tenantAccessor,
        AppDbContext dbContext)
    {
        // Endpoints públicos que não precisam de tenant
        var path = context.Request.Path.Value?.ToLower() ?? "";
        if (IsPublicEndpoint(path))
        {
            await next(context);
            return;
        }

        // Resolve o tenant
        var tenantInfo = await tenantResolver.ResolveAsync(context);

        if (tenantInfo == null)
        {
            logger.LogWarning("Tenant not resolved for request: {Path}", context.Request.Path);
            
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Tenant não identificado",
                message = "Informe o tenant via header 'X-Tenant-Id', subdomínio ou autentique-se"
            });
            return;
        }
        
        if (!tenantInfo.IsActive)
        {
            logger.LogWarning("Inactive tenant attempted access: {TenantId}", tenantInfo.TenantId);
            
            context.Response.StatusCode = 403;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Tenant inativo",
                message = "Este tenant está inativo. Entre em contato com o suporte."
            });
            return;
        }
        
        // NOVO: Verificar subscription
        var subscription = await dbContext.Subscriptions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TenantId == tenantInfo.TenantId);

        if (subscription == null || !subscription.IsValid())
        {
            logger.LogWarning("Subscription inválida para tenant: {TenantId}", tenantInfo.TenantId);
        
            context.Response.StatusCode = 402; // Payment Required
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Subscription inativa",
                message = "Sua subscription expirou. Renove para continuar usando o sistema.",
                trialExpired = subscription?.TrialEndDate < DateTime.UtcNow
            });
            return;
        }

        // Adiciona info da subscription ao TenantInfo
        // tenantInfo.SubscriptionStatus = subscription.Status.ToString();
        // tenantInfo.MaxUsers = subscription.MaxUsers;

        // Popula o TenantAccessor para uso em toda a request
        tenantAccessor.SetTenantInfo(tenantInfo);

        // Adiciona também no HttpContext.Items para fácil acesso
        context.Items["TenantInfo"] = tenantInfo;

        logger.LogDebug("Tenant resolved: {TenantName} ({TenantId})", 
            tenantInfo.TenantName, tenantInfo.TenantId);

        await next(context);
    }

    private static bool IsPublicEndpoint(string path)
    {
        var publicPaths = new[]
        {
            "/health",
            "/swagger",
            "/api/v1/tenants", 
            "/api/v1/auth/register",
            "/api/v1/auth/login",
            "/api/v1/auth/refresh"
        };

        return publicPaths.Any(p => path.StartsWith(p));
    }
}