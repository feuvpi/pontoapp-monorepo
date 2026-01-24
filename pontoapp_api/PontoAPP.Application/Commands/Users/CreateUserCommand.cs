using MediatR;
using PontoAPP.Application.DTOs.Users;

namespace PontoAPP.Application.Commands.Users;

public class CreateUserCommand : IRequest<UserResponse>
{
    public Guid TenantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string? PIS { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Employee";
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public DateTime? HiredAt { get; set; }
}
