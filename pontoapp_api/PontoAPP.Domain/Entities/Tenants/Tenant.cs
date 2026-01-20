using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.ValueObjects;

namespace PontoAPP.Domain.Entities.Tenants;

/// <summary>
/// Representa uma empresa/organização que assina o sistema
/// Armazenada no SystemDbContext (não tem TenantId)
/// </summary>
public class Tenant : BaseEntity, IAuditableEntity
{
    public string Name { get; private set; } = string.Empty; // Razão Social
    public string Slug { get; private set; } = string.Empty; // usado para resolver o tenant
    public Email Email { get; private set; } = null!;
    public CNPJ CNPJ { get; private set; } = null!; // Obrigatório
    
    // Documentos adicionais - Portaria 671
    public string? CEI { get; private set; } // Cadastro Específico do INSS (opcional - obras de construção)
    public string? InscricaoEstadual { get; private set; } // IE (opcional)
    
    public bool IsActive { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    
    // Relacionamentos
    public virtual Subscription? Subscription { get; private set; }

    // EF Constructor
    private Tenant() { }

    private Tenant(
        string name, 
        string slug, 
        Email email, 
        CNPJ cnpj,
        string? cei = null,
        string? inscricaoEstadual = null)
    {
        Name = name;
        Slug = slug.ToLowerInvariant();
        Email = email;
        CNPJ = cnpj;
        CEI = cei;
        InscricaoEstadual = inscricaoEstadual;
        IsActive = true;
    }

    public static Tenant Create(
        string name, 
        string slug, 
        string email, 
        string cnpj,
        string? cei = null,
        string? inscricaoEstadual = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tenant name is required", nameof(name));

        if (string.IsNullOrWhiteSpace(slug) || !IsValidSlug(slug))
            throw new ArgumentException("Invalid slug format", nameof(slug));

        var emailVO = Email.Create(email);
        var cnpjVO = CNPJ.Create(cnpj);

        // Validar CEI se fornecido
        if (!string.IsNullOrWhiteSpace(cei))
        {
            var cleanCEI = new string(cei.Where(char.IsDigit).ToArray());
            if (cleanCEI.Length != 12)
                throw new ArgumentException("CEI must have 12 digits", nameof(cei));
        }

        return new Tenant(name, slug, emailVO, cnpjVO, cei, inscricaoEstadual);
    }

    public void UpdateInfo(string name, string email, string? cnpj)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        if (!string.IsNullOrWhiteSpace(email))
            Email = Email.Create(email);

        if (!string.IsNullOrWhiteSpace(cnpj))
        {
            CNPJ = CNPJ.Create(cnpj);
        }
        
        UpdatedAt = DateTime.UtcNow;
    }

    public void Update(string name, string email, string cnpj)
    {
        Name = name;
        Email = Email.Value == email ? Email : Email.Create(email);
        CNPJ = CNPJ.Value == cnpj ? CNPJ : CNPJ.Create(cnpj);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateAdditionalDocuments(string? cei, string? inscricaoEstadual)
    {
        if (!string.IsNullOrWhiteSpace(cei))
        {
            var cleanCEI = new string(cei.Where(char.IsDigit).ToArray());
            if (cleanCEI.Length != 12)
                throw new ArgumentException("CEI must have 12 digits", nameof(cei));
            
            CEI = cleanCEI;
        }

        InscricaoEstadual = inscricaoEstadual;
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

    private static bool IsValidSlug(string slug)
    {
        return slug.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }
}