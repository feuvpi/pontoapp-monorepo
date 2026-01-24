using MediatR;
using PontoAPP.Application.DTOs.Tenants;

namespace PontoAPP.Application.Commands.Tenants;

/// <summary>
/// Command to create a new tenant with its own database schema
/// </summary>
public class CreateTenantCommand : IRequest<TenantResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public int TrialDays { get; set; } = 30;
    public int MaxUsers { get; set; } = 10;
}