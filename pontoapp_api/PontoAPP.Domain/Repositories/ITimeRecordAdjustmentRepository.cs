using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Repositories;

/// <summary>
/// Repository para TimeRecordAdjustments
/// Gerencia solicitações de ajuste de registros de ponto
/// </summary>
public interface ITimeRecordAdjustmentRepository : IGenericRepository<TimeRecordAdjustment>
{
    /// <summary>
    /// Busca ajuste por ID com dados relacionados
    /// </summary>
    Task<TimeRecordAdjustment?> GetByIdWithDetailsAsync(
        Guid adjustmentId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Busca ajuste pendente para um registro específico
    /// </summary>
    Task<TimeRecordAdjustment?> GetPendingAdjustmentByRecordIdAsync(
        Guid recordId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lista todos os ajustes pendentes de um tenant
    /// </summary>
    Task<List<TimeRecordAdjustment>> GetPendingAdjustmentsByTenantAsync(
        Guid tenantId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lista ajustes de um usuário específico
    /// </summary>
    Task<List<TimeRecordAdjustment>> GetAdjustmentsByUserAsync(
        Guid userId,
        AdjustmentStatus? status = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Lista ajustes por período
    /// </summary>
    Task<List<TimeRecordAdjustment>> GetAdjustmentsByDateRangeAsync(
        Guid tenantId,
        DateTime startDate,
        DateTime endDate,
        AdjustmentStatus? status = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Conta ajustes pendentes de um tenant
    /// </summary>
    Task<int> CountPendingAdjustmentsAsync(
        Guid tenantId, 
        CancellationToken cancellationToken = default);
}