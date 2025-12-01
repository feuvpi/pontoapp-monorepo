using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.ValueObjects;

namespace PontoAPP.Domain.Entities.Identity;

/// <summary>
/// Representa um usuário dentro de um tenant
/// Armazenada no TenantDbContext (schema específico do tenant)
/// </summary>
public class User : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public string FullName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string? Pin { get; private set; } // PIN numérico para terminais
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public string? BiometricHash { get; private set; } // Hash da biometria do aparelho (opcional)
    
    // Informações adicionais
    public string? EmployeeCode { get; private set; } // Matrícula
    public string? Department { get; private set; }
    public DateTime? HiredAt { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Relacionamentos
    public virtual ICollection<TimeRecord> TimeRecords { get; private set; } = new List<TimeRecord>();

    // EF Constructor
    private User() { }

    private User(
        Guid tenantId, 
        string fullName, 
        Email email, 
        string passwordHash, 
        UserRole role,
        string? employeeCode = null,
        string? department = null)
    {
        TenantId = tenantId;
        FullName = fullName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        EmployeeCode = employeeCode;
        Department = department;
    }

    public static User Create(
        Guid tenantId,
        string fullName,
        string email,
        string passwordHash,
        UserRole role = UserRole.Employee,
        string? employeeCode = null,
        string? department = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        var emailVO = Email.Create(email);

        return new User(tenantId, fullName, emailVO, passwordHash, role, employeeCode, department);
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public void SetPin(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin) || pin.Length < 4 || !pin.All(char.IsDigit))
            throw new ArgumentException("PIN must be at least 4 digits", nameof(pin));

        Pin = pin;
    }

    public void SetBiometricHash(string biometricHash)
    {
        if (string.IsNullOrWhiteSpace(biometricHash))
            throw new ArgumentException("Biometric hash is required", nameof(biometricHash));

        BiometricHash = biometricHash;
    }

    public void UpdateInfo(string? fullName, string? email, string? employeeCode, string? department)
    {
        if (!string.IsNullOrWhiteSpace(fullName))
            FullName = fullName;

        if (!string.IsNullOrWhiteSpace(email))
            Email = Email.Create(email);

        EmployeeCode = employeeCode;
        Department = department;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    public void SetHiredDate(DateTime hiredAt)
    {
        HiredAt = hiredAt;
    }
}