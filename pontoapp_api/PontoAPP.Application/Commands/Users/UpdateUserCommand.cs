using MediatR;
using PontoAPP.Application.DTOs.Users;

namespace PontoAPP.Application.Commands.Users;

public class UpdateUserCommand : IRequest<UserResponse>
{
    public Guid UserId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? EmployeeCode { get; set; }
    public string? Department { get; set; }
    public DateTime? HiredAt { get; set; }
}