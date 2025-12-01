namespace PontoAPP.Domain.Enums;

/// <summary>
/// Status de um registro de ponto (para auditoria e validação)
/// </summary>
public enum RecordStatus
{
    /// <summary>
    /// Registro válido e confirmado
    /// </summary>
    Valid = 1,
    
    /// <summary>
    /// Registro pendente de aprovação (ex: fora do horário permitido)
    /// </summary>
    Pending = 2,
    
    /// <summary>
    /// Registro rejeitado/invalidado por um gestor
    /// </summary>
    Rejected = 3,
    
    /// <summary>
    /// Registro editado manualmente (mantém auditoria)
    /// </summary>
    Edited = 4
}