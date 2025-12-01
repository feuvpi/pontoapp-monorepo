namespace PontoAPP.Domain.Enums;

/// <summary>
/// Status da assinatura de um tenant
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    /// Período de trial (gratuito por tempo limitado)
    /// </summary>
    Trial = 1,
    
    /// <summary>
    /// Assinatura ativa e paga
    /// </summary>
    Active = 2,
    
    /// <summary>
    /// Assinatura suspensa (pagamento em atraso)
    /// </summary>
    Suspended = 3,
    
    /// <summary>
    /// Assinatura cancelada pelo cliente
    /// </summary>
    Canceled = 4,
    
    /// <summary>
    /// Assinatura expirada (trial ou falta de pagamento)
    /// </summary>
    Expired = 5
}