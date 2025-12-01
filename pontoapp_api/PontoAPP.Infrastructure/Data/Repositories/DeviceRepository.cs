using Microsoft.EntityFrameworkCore;
using PontoAPP.Domain.Entities.Devices;
using PontoAPP.Domain.Repositories;
using PontoAPP.Infrastructure.Data.Context;

namespace PontoAPP.Infrastructure.Data.Repositories;

public class DeviceRepository(AppDbContext context) : GenericRepository<Device>(context), IDeviceRepository
{
    public async Task<Device?> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId, cancellationToken);
    }

    public async Task<Device?> GetByDeviceIdAndUserIdAsync(string deviceId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(d => d.DeviceId == deviceId && d.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.LastUsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Device>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(d => d.UserId == userId && d.IsActive)
            .OrderByDescending(d => d.LastUsedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeviceExistsAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(d => d.DeviceId == deviceId, cancellationToken);
    }

    public async Task<bool> IsDeviceAuthorizedAsync(string deviceId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            d => d.DeviceId == deviceId && d.UserId == userId && d.IsActive,
            cancellationToken);
    }

    public async Task<int> GetActiveDeviceCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(d => d.UserId == userId && d.IsActive, cancellationToken);
    }
}