namespace PontoAPP.Domain.Enums;

/// <summary>
/// Status de uma solicitação de ajuste de registro de ponto
/// </summary>
public enum AdjustmentStatus
{
    /// <summary>
    /// Aguardando aprovação do gestor/RH
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// Aprovado - novo registro foi criado
    /// </summary>
    Approved = 1,
    
    /// <summary>
    /// Rejeitado pelo gestor/RH
    /// </summary>
    Rejected = 2
}