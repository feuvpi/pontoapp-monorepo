using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Data.Repositories;

/// <summary>
/// Repository para TimeRecordAdjustments
/// </summary>
public class TimeRecordAdjustmentRepository : GenericRepository<TimeRecordAdjustment>, ITimeRecordAdjustmentRepository
{
    public TimeRecordAdjustmentRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<TimeRecordAdjustment?> GetByIdWithDetailsAsync(
        Guid adjustmentId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.OriginalRecord)
                .ThenInclude(r => r.User)
            .Include(a => a.AdjustmentRecord)
            .Include(a => a.Requester)
            .Include(a => a.Approver)
            .FirstOrDefaultAsync(a => a.Id == adjustmentId, cancellationToken);
    }

    public async Task<TimeRecordAdjustment?> GetPendingAdjustmentByRecordIdAsync(
        Guid recordId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                a => a.OriginalRecordId == recordId && 
                     a.Status == AdjustmentStatus.Pending,
                cancellationToken);
    }

    public async Task<List<TimeRecordAdjustment>> GetPendingAdjustmentsByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.OriginalRecord)
                .ThenInclude(r => r.User)
            .Include(a => a.Requester)
            .Where(a => a.TenantId == tenantId && a.Status == AdjustmentStatus.Pending)
            .OrderBy(a => a.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TimeRecordAdjustment>> GetAdjustmentsByUserAsync(
        Guid userId,
        AdjustmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.OriginalRecord)
            .Include(a => a.AdjustmentRecord)
            .Include(a => a.Approver)
            .Where(a => a.RequestedBy == userId);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TimeRecordAdjustment>> GetAdjustmentsByDateRangeAsync(
        Guid tenantId,
        DateTime startDate,
        DateTime endDate,
        AdjustmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(a => a.OriginalRecord)
                .ThenInclude(r => r.User)
            .Include(a => a.Requester)
            .Include(a => a.Approver)
            .Where(a => a.TenantId == tenantId &&
                       a.RequestedAt >= startDate &&
                       a.RequestedAt <= endDate);

        if (status.HasValue)
        {
            query = query.Where(a => a.Status == status.Value);
        }

        return await query
            .OrderByDescending(a => a.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountPendingAdjustmentsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(
                a => a.TenantId == tenantId && a.Status == AdjustmentStatus.Pending,
                cancellationToken);
    }
}