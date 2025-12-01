using PontoAPP.Domain.Entities.Common;

namespace PontoAPP.Domain.Entities.Tenants;

public class SubscriptionPlan : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public int MaxUsers { get; set; }
    public bool AllowBiometricValidation { get; set; } = true;
    public bool AllowGeolocation { get; set; } = true;
    public bool AllowReports { get; set; } = true;
    public bool AllowAdvancedReports { get; set; } = false;
    public bool AllowExporting { get; set; } = false;
    public bool AllowApi { get; set; } = false;
    public int DaysRetentionPeriod { get; set; } = 90; // Período de retenção de dados
    public int MaxTerminals { get; set; } = 1; // Número máximo de terminais
}
