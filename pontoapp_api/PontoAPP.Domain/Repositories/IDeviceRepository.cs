using PontoAPP.Domain.Entities.Devices;

namespace PontoAPP.Domain.Repositories;

public interface IDeviceRepository : IGenericRepository<Device>
{
    Task<Device?> GetByDeviceIdAsync(string deviceId, CancellationToken cancellationToken = default);
    Task<Device?> GetByDeviceIdAndUserIdAsync(string deviceId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetActiveByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeviceExistsAsync(string deviceId, CancellationToken cancellationToken = default);
    Task<bool> IsDeviceAuthorizedAsync(string deviceId, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetActiveDeviceCountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}