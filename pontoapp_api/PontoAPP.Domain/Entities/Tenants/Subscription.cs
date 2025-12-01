using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Entities.Tenants;

/// <summary>
/// Representa a assinatura de um tenant
/// Armazenada no SystemDbContext
/// </summary>
public class Subscription : BaseEntity
{
    public Guid TenantId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public DateTime? TrialEndDate { get; private set; }
    public int MaxUsers { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    
    // Relacionamento
    public virtual Tenant Tenant { get; private set; }

    // EF Constructor
    private Subscription() { }

    private Subscription(Guid tenantId, int trialDays, int maxUsers)
    {
        TenantId = tenantId;
        Status = SubscriptionStatus.Trial;
        StartDate = DateTime.UtcNow;
        TrialEndDate = DateTime.UtcNow.AddDays(trialDays);
        MaxUsers = maxUsers;
        MonthlyPrice = 0; // Free during trial
    }

    public static Subscription CreateTrial(Guid tenantId, int trialDays = 30, int maxUsers = 10)
    {
        if (trialDays <= 0)
            throw new ArgumentException("Trial days must be positive", nameof(trialDays));

        if (maxUsers <= 0)
            throw new ArgumentException("Max users must be positive", nameof(maxUsers));

        return new Subscription(tenantId, trialDays, maxUsers);
    }

    public void ActivatePaidSubscription(decimal monthlyPrice, int maxUsers)
    {
        Status = SubscriptionStatus.Active;
        MonthlyPrice = monthlyPrice;
        MaxUsers = maxUsers;
        TrialEndDate = null;
        EndDate = null;
    }

    public void Suspend()
    {
        if (Status == SubscriptionStatus.Active)
            Status = SubscriptionStatus.Suspended;
    }

    public void Resume()
    {
        if (Status == SubscriptionStatus.Suspended)
            Status = SubscriptionStatus.Active;
    }

    public void Cancel()
    {
        Status = SubscriptionStatus.Canceled;
        EndDate = DateTime.UtcNow;
    }

    public void Expire()
    {
        Status = SubscriptionStatus.Expired;
        EndDate = DateTime.UtcNow;
    }

    public bool IsValid()
    {
        return Status == SubscriptionStatus.Active || 
               (Status == SubscriptionStatus.Trial && TrialEndDate > DateTime.UtcNow);
    }

    public bool IsTrialExpired()
    {
        return Status == SubscriptionStatus.Trial && TrialEndDate <= DateTime.UtcNow;
    }
}