namespace PontoAPP.Infrastructure.Multitenancy;

/// <summary>
/// Implementação do TenantAccessor usando AsyncLocal para garantir thread-safety
/// AsyncLocal mantém o valor por fluxo de execução assíncrono
/// </summary>
public class TenantAccessor : ITenantAccessor
{
    private static readonly AsyncLocal<TenantInfo?> _tenantInfo = new();

    public TenantInfo? GetTenantInfo()
    {
        return _tenantInfo.Value;
    }

    public void SetTenantInfo(TenantInfo tenantInfo)
    {
        _tenantInfo.Value = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
    }
}