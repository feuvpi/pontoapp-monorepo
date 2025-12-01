using System;
using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Domain.Entities.TimeTracking;

public class Holiday : BaseEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public bool Repeats { get; set; } = false; // Se repete anualmente
    public bool IsHalfDay { get; set; } = false;
    public string Description { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty; // Para feriados regionais
    
    // Relacionamentos
    public Tenant Tenant { get; set; } = null!;
}