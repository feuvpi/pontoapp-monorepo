using MediatR;
using PontoAPP.Application.DTOs.Auth;

namespace PontoAPP.Application.Commands.Auth;

public class RefreshTokenCommand : IRequest<LoginResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}