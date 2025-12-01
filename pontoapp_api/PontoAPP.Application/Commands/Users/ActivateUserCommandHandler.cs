using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.Users;

public class ActivateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<ActivateUserCommandHandler> logger)
    : IRequestHandler<ActivateUserCommand, bool>
{
    public async Task<bool> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        user.Activate();
        
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User activated: {UserId}", user.Id);

        return true;
    }
}