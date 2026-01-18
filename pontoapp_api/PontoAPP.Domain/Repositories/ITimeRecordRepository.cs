using PontoAPP.Domain.Entities.TimeTracking;

namespace PontoAPP.Domain.Repositories;

/// <summary>
/// Repository específico para TimeRecords (TenantDbContext)
/// </summary>
public interface ITimeRecordRepository : IGenericRepository<TimeRecord>
{
    Task<IEnumerable<TimeRecord>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeRecord>> GetByUserIdAndDateRangeAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeRecord>> GetByDateRangeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    Task<TimeRecord?> GetLastRecordByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TimeRecord>> GetPendingRecordsAsync(CancellationToken cancellationToken = default);

    Task<int> GetRecordsCountByUserAndDateAsync(Guid userId, DateTime date,
        CancellationToken cancellationToken = default);


}