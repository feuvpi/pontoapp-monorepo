using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Users;

public class ResetPasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, bool>
{
    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        var passwordHash = passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(passwordHash);
        
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password reset for user: {UserId}", user.Id);

        return true;
    }
}