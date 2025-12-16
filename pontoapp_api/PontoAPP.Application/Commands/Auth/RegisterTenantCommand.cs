using MediatR;
using PontoAPP.Application.DTOs.Auth;

namespace PontoAPP.Application.Commands.Auth;

public class RegisterTenantCommand : IRequest<LoginResponse>
{
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyDocument { get; set; } = string.Empty;
    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}