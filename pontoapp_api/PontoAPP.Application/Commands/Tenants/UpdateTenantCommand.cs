using MediatR;
using PontoAPP.Application.DTOs.Tenants;

namespace PontoAPP.Application.Commands.Tenants;

public class UpdateTenantCommand : IRequest<TenantResponse>
{
    public Guid TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CompanyDocument { get; set; } = string.Empty;
}