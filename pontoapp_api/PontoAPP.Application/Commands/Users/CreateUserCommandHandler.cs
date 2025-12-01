using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Users;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Users;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ILogger<CreateUserCommandHandler> logger)
    : IRequestHandler<CreateUserCommand, UserResponse>
{
    public async Task<UserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating user: {Email} for tenant: {TenantId}", request.Email, request.TenantId);

        // Validate email uniqueness within tenant
        if (await userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ValidationException($"Email '{request.Email}' já está em uso neste tenant.");
        }

        // Validate employee code uniqueness if provided
        if (!string.IsNullOrEmpty(request.EmployeeCode) &&
            await userRepository.EmployeeCodeExistsAsync(request.EmployeeCode, cancellationToken))
        {
            throw new ValidationException($"Matrícula '{request.EmployeeCode}' já está em uso.");
        }

        // Parse role
        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new ValidationException($"Role '{request.Role}' inválido. Use: Employee, Manager ou Admin.");
        }

        // Hash password
        var passwordHash = passwordHasher.HashPassword(request.Password);

        // Create user
        var user = User.Create(
            tenantId: request.TenantId,
            fullName: request.FullName,
            email: request.Email,
            passwordHash: passwordHash,
            role: role,
            employeeCode: request.EmployeeCode,
            department: request.Department
        );

        if (request.HiredAt.HasValue)
        {
            user.SetHiredDate(request.HiredAt.Value);
        }

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User created: {UserId} - {Email}", user.Id, user.Email.Value);

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