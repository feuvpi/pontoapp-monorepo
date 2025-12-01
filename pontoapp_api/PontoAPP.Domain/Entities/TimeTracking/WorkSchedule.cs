using System;
using System.Collections.Generic;
using PontoAPP.Domain.Entities.Common;
using PontoAPP.Domain.Entities.Identity;
using PontoAPP.Domain.Entities.Tenants;

namespace PontoAPP.Domain.Entities.TimeTracking;

public class WorkSchedule : BaseEntity
{
    public int TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Horários padrão
    public TimeSpan WorkStart { get; set; } = new TimeSpan(9, 0, 0); // 9:00 AM
    public TimeSpan WorkEnd { get; set; } = new TimeSpan(18, 0, 0); // 6:00 PM
    public int WorkDurationMinutes { get; set; } = 480; // 8 horas = 480 minutos
    public int BreakDurationMinutes { get; set; } = 60; // 1 hora = 60 minutos
    
    // Flexibilidade
    public bool IsFlexible { get; set; } = false;
    public int FlexibilityStartMinutes { get; set; } = 0; // Minutos de flexibilidade antes
    public int FlexibilityEndMinutes { get; set; } = 0; // Minutos de flexibilidade depois
    
    // Dias de trabalho
    public bool WorksOnSunday { get; set; } = false;
    public bool WorksOnMonday { get; set; } = true;
    public bool WorksOnTuesday { get; set; } = true;
    public bool WorksOnWednesday { get; set; } = true;
    public bool WorksOnThursday { get; set; } = true;
    public bool WorksOnFriday { get; set; } = true;
    public bool WorksOnSaturday { get; set; } = false;
    
    // Relacionamentos
    public Tenant Tenant { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}

