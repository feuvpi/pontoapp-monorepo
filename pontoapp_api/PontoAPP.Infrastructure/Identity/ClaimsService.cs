using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Infrastructure.Identity;

/// <summary>
/// Service for extracting and managing user claims from HttpContext
/// Provides type-safe access to authenticated user information
/// </summary>
public class ClaimsService : IClaimsService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ClaimsService> _logger;

    public ClaimsService(IHttpContextAccessor httpContextAccessor, ILogger<ClaimsService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user's ID from claims
    /// </summary>
    public Guid? GetCurrentUserId()
    {
        var userIdClaim = GetClaim("userId") ?? GetClaim(ClaimTypes.NameIdentifier);
        
        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }

    /// <summary>
    /// Gets the current tenant ID from claims
    /// </summary>
    public Guid? GetCurrentTenantId()
    {
        var tenantIdClaim = GetClaim("tenantId");
        
        if (tenantIdClaim != null && Guid.TryParse(tenantIdClaim, out var tenantId))
            return tenantId;

        return null;
    }

    /// <summary>
    /// Gets the current user's email from claims
    /// </summary>
    public string? GetCurrentUserEmail()
    {
        return GetClaim(ClaimTypes.Email) ?? GetClaim("email");
    }

    /// <summary>
    /// Gets the current user's role from claims
    /// </summary>
    public string? GetCurrentUserRole()
    {
        return GetClaim(ClaimTypes.Role) ?? GetClaim("role");
    }

    /// <summary>
    /// Gets the current user's role as enum
    /// </summary>
    public UserRole? GetCurrentUserRoleEnum()
    {
        var role = GetCurrentUserRole();
        
        if (string.IsNullOrWhiteSpace(role))
            return null;

        if (Enum.TryParse<UserRole>(role, ignoreCase: true, out var userRole))
            return userRole;

        return null;
    }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    public bool IsInRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            return false;

        var currentRole = GetCurrentUserRole();
        return string.Equals(currentRole, role, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Checks if the current user is an Admin
    /// </summary>
    public bool IsAdmin()
    {
        return IsInRole(UserRole.Admin.ToString());
    }

    /// <summary>
    /// Checks if the current user is a Manager
    /// </summary>
    public bool IsManager()
    {
        var role = GetCurrentUserRoleEnum();
        return role == UserRole.Admin || role == UserRole.Manager;
    }

    /// <summary>
    /// Gets all claims for the current user
    /// </summary>
    public IEnumerable<Claim> GetAllClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? Enumerable.Empty<Claim>();
    }

    /// <summary>
    /// Gets a specific claim value by type
    /// </summary>
    public string? GetClaim(string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return null;

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return null;

        return user.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Gets all values for a specific claim type (for multi-valued claims)
    /// </summary>
    public IEnumerable<string> GetClaimValues(string claimType)
    {
        if (string.IsNullOrWhiteSpace(claimType))
            return Enumerable.Empty<string>();

        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null)
            return Enumerable.Empty<string>();

        return user.FindAll(claimType).Select(c => c.Value);
    }

    /// <summary>
    /// Validates that the current user belongs to the specified tenant
    /// </summary>
    public bool BelongsToTenant(Guid tenantId)
    {
        var currentTenantId = GetCurrentTenantId();
        return currentTenantId.HasValue && currentTenantId.Value == tenantId;
    }

    /// <summary>
    /// Gets the current user's full identity information
    /// </summary>
    public UserIdentity? GetCurrentUserIdentity()
    {
        if (!IsAuthenticated())
            return null;

        var userId = GetCurrentUserId();
        var tenantId = GetCurrentTenantId();

        if (!userId.HasValue || !tenantId.HasValue)
        {
            _logger.LogWarning("User is authenticated but userId or tenantId is missing from claims");
            return null;
        }

        return new UserIdentity
        {
            UserId = userId.Value,
            TenantId = tenantId.Value,
            Email = GetCurrentUserEmail(),
            Role = GetCurrentUserRole(),
            RoleEnum = GetCurrentUserRoleEnum()
        };
    }
}

/// <summary>
/// Interface for claims-based operations
/// </summary>
public interface IClaimsService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Guid? GetCurrentUserId();

    /// <summary>
    /// Gets the current tenant ID
    /// </summary>
    Guid? GetCurrentTenantId();

    /// <summary>
    /// Gets the current user's email
    /// </summary>
    string? GetCurrentUserEmail();

    /// <summary>
    /// Gets the current user's role
    /// </summary>
    string? GetCurrentUserRole();

    /// <summary>
    /// Gets the current user's role as enum
    /// </summary>
    UserRole? GetCurrentUserRoleEnum();

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated();

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    bool IsInRole(string role);

    /// <summary>
    /// Checks if the current user is an Admin
    /// </summary>
    bool IsAdmin();

    /// <summary>
    /// Checks if the current user is a Manager
    /// </summary>
    bool IsManager();

    /// <summary>
    /// Gets all claims for the current user
    /// </summary>
    IEnumerable<Claim> GetAllClaims();

    /// <summary>
    /// Gets a specific claim value
    /// </summary>
    string? GetClaim(string claimType);

    /// <summary>
    /// Gets all values for a claim type
    /// </summary>
    IEnumerable<string> GetClaimValues(string claimType);

    /// <summary>
    /// Validates that the current user belongs to a tenant
    /// </summary>
    bool BelongsToTenant(Guid tenantId);

    /// <summary>
    /// Gets the current user's full identity
    /// </summary>
    UserIdentity? GetCurrentUserIdentity();
}

/// <summary>
/// Represents the authenticated user's identity information
/// </summary>
public class UserIdentity
{
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string? Email { get; set; }
    public string? Role { get; set; }
    public UserRole? RoleEnum { get; set; }
}