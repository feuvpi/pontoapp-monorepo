// PontoAPP.Application/Commands/Auth/RefreshTokenCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Logging;
using PontoAPP.Application.DTOs.Auth;
using PontoAPP.Application.Exceptions;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Identity;

namespace PontoAPP.Application.Commands.Auth;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        IJwtTokenService jwtTokenService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (user == null)
        {
            throw new BusinessException("Token inválido.");
        }

        if (user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            throw new BusinessException("Token expirado. Faça login novamente.");
        }

        if (!user.IsActive)
        {
            throw new BusinessException("Usuário inativo.");
        }

        var tenant = await _tenantRepository.GetByIdAsync(user.TenantId, cancellationToken);
        if (tenant == null || !tenant.IsActive)
        {
            throw new BusinessException("Empresa inativa.");
        }

        var token = _jwtTokenService.GenerateAccessToken(
            userId: user.Id,
            tenantId: tenant.Id,
            email: user.Email.Value,
            role: user.Role.ToString(),
            additionalClaims: new Dictionary<string, string>
            {
                { "name", user.FullName },
                { "must_change_password", user.MustChangePassword.ToString().ToLower() }
            }
        );

        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
        var expiresAt = _jwtTokenService.GetTokenExpirationTime(token) ?? DateTime.UtcNow.AddHours(1);

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new LoginResponse
        {
            Token = token,
            RefreshToken = newRefreshToken,
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