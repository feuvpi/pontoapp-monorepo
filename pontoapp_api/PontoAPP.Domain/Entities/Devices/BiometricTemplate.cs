using System;
using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Entities.Tenants;
using PontoAPP.Domain.Enums;

namespace PontoAPP.Domain.Entities.Devices;

public class BiometricTemplate : BaseEntity
{
    public int TenantId { get; set; }
    public int UserId { get; set; }
    public BiometricTemplateType TemplateType { get; set; } = BiometricTemplateType.FacialVector;
    public byte[] TemplateData { get; set; } = Array.Empty<byte>();
    public byte[] Salt { get; set; } = Array.Empty<byte>(); // Para incrementar segurança
    public string Algorithm { get; set; } = "FACIAL_EIGENFACES"; // Algoritmo usado para gerar o template
    public string Version { get; set; } = "1.0"; // Versão do algoritmo
    public bool Active { get; set; } = true;
    
    // Propriedades de navegação
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}


