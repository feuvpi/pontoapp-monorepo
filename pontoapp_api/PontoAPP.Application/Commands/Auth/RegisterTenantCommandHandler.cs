using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Auth;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Data.Context;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Auth;

public class RegisterTenantCommandHandler(
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    ILogger<RegisterTenantCommandHandler> logger)
    : IRequestHandler<RegisterTenantCommand, LoginResponse>
{
    public async Task<LoginResponse> Handle(RegisterTenantCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registering new tenant: {CompanyName}", request.CompanyName);

        // Validar email único
        var existingUser = await userRepository.GetByEmailAsync(request.AdminEmail, cancellationToken);
        if (existingUser != null)
        {
            throw new ValidationException("Este e-mail já está cadastrado.");
        }

        // Gerar slug
        var slug = GenerateSlug(request.CompanyName);
        if (await tenantRepository.SlugExistsAsync(slug, cancellationToken))
        {
            slug = $"{slug}-{Guid.NewGuid().ToString()[..8]}";
        }

        // Criar Tenant
        var tenant = Tenant.Create(
            name: request.CompanyName,
            slug: slug,
            email: request.AdminEmail,
            companyDocument: request.CompanyDocument
        );

        // Criar Subscription (trial)
        var subscription = Subscription.CreateTrial(
            tenantId: tenant.Id,
            trialDays: 14,
            maxUsers: 10
        );

        // Criar admin
        var passwordHash = passwordHasher.HashPassword(request.Password);
        var admin = User.CreateAdmin(
            tenantId: tenant.Id,
            fullName: request.AdminName,
            email: request.AdminEmail,
            passwordHash: passwordHash
        );

        // Gerar tokens
        var token = jwtTokenService.GenerateAccessToken(
            userId: admin.Id,
            tenantId: tenant.Id,
            email: admin.Email.Value,
            role: admin.Role.ToString(),
            additionalClaims: new Dictionary<string, string>
            {
                { "name", admin.FullName },
                { "must_change_password", "false" }
            }
        );

        var refreshToken = jwtTokenService.GenerateRefreshToken();
        var expiresAt = jwtTokenService.GetTokenExpirationTime(token) ?? DateTime.UtcNow.AddHours(1);

        admin.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(7));
        admin.RecordLogin();

        try
        {
            await tenantRepository.AddAsync(tenant, cancellationToken);
            await dbContext.Subscriptions.AddAsync(subscription, cancellationToken);
            await dbContext.Users.AddAsync(admin, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Tenant registered: {TenantId}", tenant.Id);

            return new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                TenantId = tenant.Id.ToString(),
                User = new UserInfo
                {
                    Id = admin.Id.ToString(),
                    FullName = admin.FullName,
                    Email = admin.Email.Value,
                    Role = admin.Role.ToString(),
                    IsActive = admin.IsActive,
                    MustChangePassword = admin.MustChangePassword,
                    EmployeeCode = admin.EmployeeCode,
                    Department = admin.Department
                }
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error registering tenant: {CompanyName}", request.CompanyName);
            throw new BusinessException("Erro ao criar conta. Tente novamente.");
        }
    }

    private static string GenerateSlug(string name)
    {
        return name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("&", "e");
    }
}