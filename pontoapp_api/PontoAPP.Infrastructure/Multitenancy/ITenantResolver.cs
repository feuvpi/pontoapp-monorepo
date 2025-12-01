using Microsoft.AspNetCore.Http;

namespace PontoAPP.Infrastructure.Multitenancy;

/// <summary>
/// Resolve o tenant baseado em informações da requisição HTTP
/// </summary>
public interface ITenantResolver
{
    Task<TenantInfo?> ResolveAsync(HttpContext httpContext);
}