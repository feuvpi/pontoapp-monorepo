namespace PontoAPP.Domain.Entities.Common;

/// <summary>
/// Interface para entidades que precisam rastrear criação e modificação
/// </summary>
public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedAt { get; }
    string? UpdatedBy { get; set; }
    DateTime? UpdatedAt { get; }
}