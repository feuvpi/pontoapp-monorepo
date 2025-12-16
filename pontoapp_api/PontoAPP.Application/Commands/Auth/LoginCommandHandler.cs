using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Auth;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Auth;

public class LoginCommandHandler(
    IUserRepository userRepository,
    ITenantRepository tenantRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email: {Email}", request.Email);

        // Buscar usuário por email (ignora filtro de tenant)
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User not found: {Email}", request.Email);
            throw new BusinessException("E-mail ou senha inválidos.");
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Inactive user attempted login: {Email}", request.Email);
            throw new BusinessException("Usuário inativo. Contate o administrador.");
        }

        // Verificar senha
        if (!passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Invalid password for user: {Email}", request.Email);
            throw new BusinessException("E-mail ou senha inválidos.");
        }

        // Buscar tenant
        var tenant = await tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant == null || !tenant.IsActive)
        {
            throw new BusinessException("Empresa inativa. Contate o administrador.");
        }

        // Gerar tokens usando a interface existente
        var additionalClaims = new Dictionary<string, string>
        {
            { "name", user.FullName },
            { "must_change_password", user.MustChangePassword.ToString().ToLower() }
        };

        var token = jwtTokenService.GenerateAccessToken(
            userId: user.Id,
            tenantId: tenant.Id,
            email: user.Email.Value,
            role: user.Role.ToString(),
            additionalClaims: additionalClaims
        );

        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var expiresAt = jwtTokenService.GetTokenExpirationTime(token) ?? DateTime.UtcNow.AddHours(1);

        // Salvar refresh token e registrar login
        user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        user.RecordLogin();
        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        logger.LogInformation("User logged in successfully: {Email}", request.Email);

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            TenantId = tenant.Id.ToString(),
            User = new UserInfo
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email.Value,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                MustChangePassword = user.MustChangePassword,
                EmployeeCode = user.EmployeeCode,
                Department = user.Department
            }
        };
    }
}