using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Data.Repositories;

/// <summary>
/// Repository para TimeRecords (TenantDbContext)
/// </summary>
public class TimeRecordRepository : GenericRepository<TimeRecord>, ITimeRecordRepository
{
    public TimeRecordRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TimeRecord>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeRecord>> GetByUserIdAndDateRangeAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId &&
                        tr.RecordedAt >= startDate &&
                        tr.RecordedAt <= endDate)
            .OrderByDescending(tr => tr.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeRecord>> GetByDateRangeAsync(
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tr => tr.User)
            .Where(tr => tr.RecordedAt >= startDate && tr.RecordedAt <= endDate)
            .OrderByDescending(tr => tr.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TimeRecord?> GetLastRecordByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(tr => tr.UserId == userId)
            .OrderByDescending(tr => tr.RecordedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TimeRecord>> GetPendingRecordsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(tr => tr.User)
            .Where(tr => tr.Status == RecordStatus.Pending)
            .OrderBy(tr => tr.RecordedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetRecordsCountByUserAndDateAsync(
        Guid userId,
        DateTime date,
        CancellationToken cancellationToken = default)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .CountAsync(tr => tr.UserId == userId &&
                             tr.RecordedAt >= startOfDay &&
                             tr.RecordedAt < endOfDay,
                cancellationToken);
    }
}