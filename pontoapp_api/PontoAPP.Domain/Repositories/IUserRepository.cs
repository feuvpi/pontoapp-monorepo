using PontoAPP.Domain.Entities.Identity;

namespace PontoAPP.Domain.Repositories;

/// <summary>
/// Repository específico para Users (TenantDbContext)
/// </summary>
public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByRoleAsync(Enums.UserRole role, CancellationToken cancellationToken = default);
    Task<int> GetActiveUsersCountAsync(CancellationToken cancellationToken = default);
}