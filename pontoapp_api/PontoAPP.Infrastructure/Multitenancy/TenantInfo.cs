namespace PontoAPP.Infrastructure.Multitenancy;

/// <summary>
/// Informações do tenant resolvido na requisição atual
/// </summary>
public class TenantInfo
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public TenantInfo() { }

    public TenantInfo(Guid tenantId, string tenantName, string slug, bool isActive = true)
    {
        TenantId = tenantId;
        TenantName = tenantName;
        Slug = slug;
        IsActive = isActive;
    }
}