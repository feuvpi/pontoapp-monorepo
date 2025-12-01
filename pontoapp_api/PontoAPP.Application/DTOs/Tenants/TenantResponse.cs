using PontoAPP.Domain.Enums;

namespace PontoAPP.Application.DTOs.Tenants;

/// <summary>
/// Response containing tenant information
/// </summary>
public class TenantResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? CompanyDocument { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // Subscription info
    public SubscriptionInfo? Subscription { get; set; }
}

/// <summary>
/// Subscription information for a tenant
/// </summary>
public class SubscriptionInfo
{
    public Guid Id { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int MaxUsers { get; set; }
    public decimal MonthlyPrice { get; set; }
    public bool IsValid { get; set; }
}