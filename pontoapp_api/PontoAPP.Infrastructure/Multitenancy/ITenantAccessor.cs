namespace PontoAPP.Infrastructure.Multitenancy;

/// <summary>
/// Accessor para obter informações do tenant da requisição atual
/// </summary>
public interface ITenantAccessor
{
    TenantInfo? GetTenantInfo();
    void SetTenantInfo(TenantInfo tenantInfo);
}