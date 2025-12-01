using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.ValueObjects;

namespace PontoAPP.Domain.Entities.Tenants;

/// <summary>
/// Representa uma empresa/organização que assina o sistema
/// Armazenada no SystemDbContext (não tem TenantId)
/// </summary>
public class Tenant : BaseEntity, IAuditableEntity
{
    public string Name { get; private set; }
    public string Slug { get; private set; } // usado para resolver o tenant (ex: empresa.pontoapp.com)
    public Email Email { get; private set; }
    public string? CompanyDocument { get; private set; } // CNPJ
    public bool IsActive { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Relacionamentos
    public virtual Subscription? Subscription { get; private set; }

    // EF Constructor
    private Tenant() { }

    private Tenant(string name, string slug, Email email, string? companyDocument)
    {
        Name = name;
        Slug = slug.ToLowerInvariant();
        Email = email;
        CompanyDocument = companyDocument;
        IsActive = true;
    }

    public static Tenant Create(string name, string slug, string email, string? companyDocument = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(slug) || !IsValidSlug(slug))
            throw new ArgumentException("Invalid slug format", nameof(slug));

        var emailVO = Email.Create(email);

        return new Tenant(name, slug, emailVO, companyDocument);
    }

    public void UpdateInfo(string name, string email, string? companyDocument)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(email))
            Email = Email.Create(email);

        CompanyDocument = companyDocument;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private static bool IsValidSlug(string slug)
    {
        return slug.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }
}