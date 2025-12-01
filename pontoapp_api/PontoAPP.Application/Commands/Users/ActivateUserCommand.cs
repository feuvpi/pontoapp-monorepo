using MediatR;

namespace PontoAPP.Application.Commands.Users;

public class ActivateUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}
