using MediatR;
using PontoAPP.Application.DTOs.Auth;

namespace PontoAPP.Application.Commands.Auth;

public class LoginCommand : IRequest<LoginResponse>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}