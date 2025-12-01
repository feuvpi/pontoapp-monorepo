using MediatR;

namespace PontoAPP.Application.Commands.Users;

public class DeactivateUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}