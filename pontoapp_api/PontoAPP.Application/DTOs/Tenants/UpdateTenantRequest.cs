namespace PontoAPP.Application.DTOs.Tenants;

public class UpdateTenantRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyDocument { get; set; } = string.Empty;
}