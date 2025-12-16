// PontoAPP.Domain/Repositories/IUserRepository.cs
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    /// <summary>
    /// Busca usuário por email dentro do tenant atual
    /// </summary>
    Task<User?> GetByEmailInTenantAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca usuário por código de matrícula
    /// </summary>
    Task<User?> GetByEmployeeCodeAsync(string employeeCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se email existe no tenant atual (para validação)
    /// </summary>
    Task<bool> EmailExistsInTenantAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se email existe (global, ignora tenant)
    /// </summary>
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se código de matrícula existe no tenant
    /// </summary>
    Task<bool> EmployeeCodeExistsAsync(string employeeCode, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna todos os usuários ativos do tenant
    /// </summary>
    Task<IEnumerable<User>> GetActiveUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna usuários por role no tenant
    /// </summary>
    Task<IEnumerable<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Conta usuários ativos no tenant
    /// </summary>
    Task<int> GetActiveUserCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca usuário por email IGNORANDO filtro de tenant (para login)
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca usuário por refresh token IGNORANDO filtro de tenant
    /// </summary>
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}