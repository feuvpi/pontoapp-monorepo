using MediatR;

namespace PontoAPP.Application.Commands.Auth;

public class ChangePasswordCommand : IRequest
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}