using MediatR;

namespace PontoAPP.Application.Commands.Users;

public class ResetPasswordCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}
