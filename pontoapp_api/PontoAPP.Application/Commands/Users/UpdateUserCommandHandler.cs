// PontoAPP.Application/Commands/Users/UpdateUserCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Users;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;

namespace PontoAPP.Application.Commands.Users;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new NotFoundException($"Usuário com ID '{request.UserId}' não encontrado.");
        }

        // Validate email uniqueness if changing
        if (!string.IsNullOrEmpty(request.Email) && 
            request.Email.ToLowerInvariant() != user.Email.Value.ToLowerInvariant())
        {
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                throw new ValidationException($"Email '{request.Email}' já está em uso.");
            }
        }

        // Validate employee code uniqueness if changing
        if (!string.IsNullOrEmpty(request.EmployeeCode) && 
            request.EmployeeCode != user.EmployeeCode)
        {
            if (await _userRepository.EmployeeCodeExistsAsync(request.EmployeeCode, cancellationToken))
            {
                throw new ValidationException($"Matrícula '{request.EmployeeCode}' já está em uso.");
            }
        }

        // Update basic info
        user.UpdateInfo(request.FullName, request.Email, request.EmployeeCode, request.Department);

        // Update role if provided
        if (!string.IsNullOrEmpty(request.Role))
        {
            if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
            {
                throw new ValidationException($"Role '{request.Role}' inválido.");
            }
            user.ChangeRole(role);
        }

        // Update hired date if provided
        if (request.HiredAt.HasValue)
        {
            user.SetHiredDate(request.HiredAt.Value);
        }

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User updated: {UserId}", user.Id);

        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email.Value,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            EmployeeCode = user.EmployeeCode,
            Department = user.Department,
            HiredAt = user.HiredAt,
            CreatedAt = user.CreatedAt
        };
    }
}