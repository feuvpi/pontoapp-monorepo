namespace PontoAPP.Application.DTOs.Tenants;

/// <summary>
/// Request to create a new tenant (company)
/// </summary>
public class CreateTenantRequest
{
    /// <summary>
    /// Company name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique slug for tenant identification (used in subdomain or tenant resolution)
    /// Example: "acme-corp" → acme-corp.pontoapp.com
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Company email (will be used for admin user)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Company document (CNPJ in Brazil)
    /// Optional during registration
    /// </summary>
    public string CompanyDocument { get; set; } = string.Empty;
}