using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.TimeTracking;
using PontoAPP.Domain.Enums;
using PontoAPP.Domain.ValueObjects;

namespace PontoAPP.Domain.Entities.Identity;

/// <summary>
/// Representa um usuário (funcionário) do sistema
/// </summary>
public class User : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public string FullName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = string.Empty;
    public string? Pin { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }
    public string? BiometricHash { get; private set; }
    
    // Documentos - Portaria 671
    public CPF? CPF { get; private set; } = null!; // Obrigatório
    public PIS? PIS { get; private set; } // Opcional (por enquanto)
    
    // Informações adicionais
    public string? EmployeeCode { get; private set; }
    public string? Department { get; private set; }
    public DateTime? HiredAt { get; private set; }
    
    // Auth
    public bool MustChangePassword { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    
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
        CPF? cpf,
        PIS? pis,
        string passwordHash, 
        UserRole role,
        bool mustChangePassword,
        string? employeeCode = null,
        string? department = null)
    {
        TenantId = tenantId;
        FullName = fullName;
        Email = email;
        CPF = cpf;
        PIS = pis;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
        MustChangePassword = mustChangePassword;
        EmployeeCode = employeeCode;
        Department = department;
    }

    public static User Create(
        Guid tenantId,
        string fullName,
        string email,
        string cpf,
        string? pis, // Opcional
        string passwordHash,
        UserRole role = UserRole.Employee,
        bool mustChangePassword = false,
        string? employeeCode = null,
        string? department = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));
        CPF? cpfVO = null;
        var emailVO = Email.Create(email);
        if (!string.IsNullOrWhiteSpace(cpf))
            cpfVO = CPF.Create(cpf);
        
        // PIS é opcional
        PIS? pisVO = null;
        if (!string.IsNullOrWhiteSpace(pis))
        {
            pisVO = PIS.Create(pis);
        }

        return new User(
            tenantId, 
            fullName, 
            emailVO, 
            cpfVO, 
            pisVO, 
            passwordHash, 
            role, 
            mustChangePassword, 
            employeeCode, 
            department);
    }

    /// <summary>
    /// Cria usuário admin para um novo tenant
    /// </summary>
    public static User CreateAdmin(
        Guid tenantId,
        string fullName,
        string email,
        string cpf,
        string? pis,
        string passwordHash)
    {
        return Create(
            tenantId: tenantId,
            fullName: fullName,
            email: email,
            cpf: cpf,
            pis: pis,
            passwordHash: passwordHash,
            role: UserRole.Admin,
            mustChangePassword: false
        );
    }

    /// <summary>
    /// Cria funcionário (chamado pelo admin)
    /// </summary>
    public static User CreateEmployee(
        Guid tenantId,
        string fullName,
        string email,
        string cpf,
        string? pis,
        string passwordHash,
        string? employeeCode = null,
        string? department = null)
    {
        return Create(
            tenantId: tenantId,
            fullName: fullName,
            email: email,
            cpf: cpf,
            pis: pis,
            passwordHash: passwordHash,
            role: UserRole.Employee,
            mustChangePassword: true,
            employeeCode: employeeCode,
            department: department
        );
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        MustChangePassword = false;
    }

    public void SetRefreshToken(string token, DateTime expiresAt)
    {
        RefreshToken = token;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void ClearRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiresAt = null;
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
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
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDocuments(string? cpf, string? pis)
    {
        if (!string.IsNullOrWhiteSpace(cpf))
        {
            CPF = CPF.Create(cpf);
        }

        if (!string.IsNullOrWhiteSpace(pis))
        {
            PIS = PIS.Create(pis);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica se o usuário tem todos os documentos necessários para bater ponto
    /// (Para exportação de AFD, PIS é obrigatório)
    /// </summary>
    public bool HasRequiredDocuments()
    {
        return PIS != null;
    }

    /// <summary>
    /// Verifica se pode exportar dados fiscais (AFD)
    /// </summary>
    public bool CanExportToAFD()
    {
        return HasRequiredDocuments() && IsActive;
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate() 
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Deactivate() 
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetHiredDate(DateTime hiredAt)
    {
        HiredAt = hiredAt;
        UpdatedAt = DateTime.UtcNow;
    }
}