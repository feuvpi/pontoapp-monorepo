using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace PontoAPP.Infrastructure.Identity;

/// <summary>
/// Service for generating and validating JWT tokens
/// Implements secure token generation with proper claims and refresh token support
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly JwtSecurityTokenHandler _tokenHandler;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _tokenHandler = new JwtSecurityTokenHandler();
        _tokenValidationParameters = CreateTokenValidationParameters();
    }

    /// <summary>
    /// Generates a JWT access token for a user
    /// </summary>
    public string GenerateAccessToken(Guid userId, Guid tenantId, string email, string role, Dictionary<string, string>? additionalClaims = null)
    {
        var secret = _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");

        var issuer = _configuration["JwtSettings:Issuer"] ?? "PontoAPP";
        var audience = _configuration["JwtSettings:Audience"] ?? "PontoAPP-Users";
        var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("userId", userId.ToString()),
            new("tenantId", tenantId.ToString()),
            new("role", role)
        };

        // Add any additional claims
        if (additionalClaims != null)
        {
            foreach (var claim in additionalClaims)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenString = _tokenHandler.WriteToken(token);

        _logger.LogDebug("Generated JWT token for user {UserId} in tenant {TenantId}", userId, tenantId);

        return tokenString;
    }

    /// <summary>
    /// Generates a secure refresh token
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates a JWT token and returns the principal
    /// </summary>
    public ClaimsPrincipal? ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var principal = _tokenHandler.ValidateToken(token, _tokenValidationParameters, out var securityToken);

            // Verify it's a JWT token with correct algorithm
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.LogWarning("Invalid token algorithm");
                return null;
            }

            return principal;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogDebug("Token expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("Invalid token signature");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return null;
        }
    }

    /// <summary>
    /// Extracts user ID from token without full validation (for refresh token scenarios)
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == JwtRegisteredClaimNames.Sub);
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting user ID from token");
        }

        return null;
    }

    /// <summary>
    /// Extracts tenant ID from token
    /// </summary>
    public Guid? GetTenantIdFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            var tenantIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "tenantId");
            
            if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim.Value, out var tenantId))
                return tenantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting tenant ID from token");
        }

        return null;
    }

    /// <summary>
    /// Gets token expiration time
    /// </summary>
    public DateTime? GetTokenExpirationTime(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var jwtToken = _tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting token expiration");
            return null;
        }
    }

    /// <summary>
    /// Creates token validation parameters from configuration
    /// </summary>
    private TokenValidationParameters CreateTokenValidationParameters()
    {
        var secret = _configuration["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret not configured");

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidAudience = _configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
        };
    }
}

/// <summary>
/// Interface for JWT token operations
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT access token for a user
    /// </summary>
    string GenerateAccessToken(Guid userId, Guid tenantId, string email, string role, Dictionary<string, string>? additionalClaims = null);

    /// <summary>
    /// Generates a secure refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token and returns the claims principal
    /// </summary>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Extracts user ID from token
    /// </summary>
    Guid? GetUserIdFromToken(string token);

    /// <summary>
    /// Extracts tenant ID from token
    /// </summary>
    Guid? GetTenantIdFromToken(string token);

    /// <summary>
    /// Gets token expiration time
    /// </summary>
    DateTime? GetTokenExpirationTime(string token);
}