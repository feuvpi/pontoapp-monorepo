using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Auth;

public class ChangePasswordCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<ChangePasswordCommandHandler> logger)
    : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Changing password for user: {UserId}", request.UserId);

        // Get user
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("Usuário não encontrado");
        }

        // Verify current password
        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            logger.LogWarning("Invalid current password for user: {UserId}", request.UserId);
            throw new ValidationException("Senha atual incorreta");
        }

        // Validate new password
        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            throw new ValidationException("Nova senha deve ter pelo menos 6 caracteres");
        }

        // Hash new password
        var newPasswordHash = passwordHasher.HashPassword(request.NewPassword);
        
        // Update password (você precisa ter esse método no User entity)
        user.UpdatePassword(newPasswordHash);
        
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Password changed successfully for user: {UserId}", request.UserId);
    }
}