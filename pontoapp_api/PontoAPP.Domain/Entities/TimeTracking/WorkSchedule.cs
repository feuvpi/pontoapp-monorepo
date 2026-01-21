using PontoAPP.Domain.Entities.Common;

namespace PontoAPP.Domain.Entities.TimeTracking;

/// <summary>
/// Representa uma jornada de trabalho (horários e dias da semana)
/// Usado para calcular horas extras, atrasos, etc (Portaria 671)
/// </summary>
public class WorkSchedule : BaseEntity, ITenantEntity, IAuditableEntity
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty; // "Comercial 44h", "Turno Noite"
    public string? Description { get; private set; }
    public WorkScheduleType Type { get; private set; }
    public decimal WeeklyHours { get; private set; } // 44, 40, 36, etc
    
    // Segunda-feira
    public TimeSpan? MondayStart { get; private set; }
    public TimeSpan? MondayEnd { get; private set; }
    public int MondayBreakMinutes { get; private set; }
    
    // Terça-feira
    public TimeSpan? TuesdayStart { get; private set; }
    public TimeSpan? TuesdayEnd { get; private set; }
    public int TuesdayBreakMinutes { get; private set; }
    
    // Quarta-feira
    public TimeSpan? WednesdayStart { get; private set; }
    public TimeSpan? WednesdayEnd { get; private set; }
    public int WednesdayBreakMinutes { get; private set; }
    
    // Quinta-feira
    public TimeSpan? ThursdayStart { get; private set; }
    public TimeSpan? ThursdayEnd { get; private set; }
    public int ThursdayBreakMinutes { get; private set; }
    
    // Sexta-feira
    public TimeSpan? FridayStart { get; private set; }
    public TimeSpan? FridayEnd { get; private set; }
    public int FridayBreakMinutes { get; private set; }
    
    // Sábado
    public TimeSpan? SaturdayStart { get; private set; }
    public TimeSpan? SaturdayEnd { get; private set; }
    public int SaturdayBreakMinutes { get; private set; }
    
    // Domingo
    public TimeSpan? SundayStart { get; private set; }
    public TimeSpan? SundayEnd { get; private set; }
    public int SundayBreakMinutes { get; private set; }
    
    // Configurações
    public int ToleranceMinutes { get; private set; } // Tolerância de atraso (ex: 10min)
    public bool IsActive { get; private set; }
    
    // Auditoria
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // EF Constructor
    private WorkSchedule() { }

    private WorkSchedule(
        Guid tenantId,
        string name,
        WorkScheduleType type,
        decimal weeklyHours,
        int toleranceMinutes = 10,
        string? description = null)
    {
        TenantId = tenantId;
        Name = name;
        Description = description;
        Type = type;
        WeeklyHours = weeklyHours;
        ToleranceMinutes = toleranceMinutes;
        IsActive = true;
    }

    /// <summary>
    /// Cria uma jornada de trabalho
    /// </summary>
    public static WorkSchedule Create(
        Guid tenantId,
        string name,
        WorkScheduleType type,
        decimal weeklyHours,
        int toleranceMinutes = 10,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required", nameof(name));

        if (weeklyHours <= 0 || weeklyHours > 60)
            throw new ArgumentException("Weekly hours must be between 1 and 60", nameof(weeklyHours));

        if (toleranceMinutes < 0 || toleranceMinutes > 60)
            throw new ArgumentException("Tolerance must be between 0 and 60 minutes", nameof(toleranceMinutes));

        return new WorkSchedule(tenantId, name, type, weeklyHours, toleranceMinutes, description);
    }

    /// <summary>
    /// Define horário de trabalho para um dia da semana
    /// </summary>
    public void SetDaySchedule(
        DayOfWeek dayOfWeek,
        TimeSpan? start,
        TimeSpan? end,
        int breakMinutes = 0)
    {
        if (start.HasValue && end.HasValue && start >= end)
            throw new ArgumentException("End time must be after start time");

        if (breakMinutes < 0 || breakMinutes > 480) // Máximo 8h de intervalo
            throw new ArgumentException("Break minutes must be between 0 and 480", nameof(breakMinutes));

        switch (dayOfWeek)
        {
            case DayOfWeek.Monday:
                MondayStart = start;
                MondayEnd = end;
                MondayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Tuesday:
                TuesdayStart = start;
                TuesdayEnd = end;
                TuesdayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Wednesday:
                WednesdayStart = start;
                WednesdayEnd = end;
                WednesdayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Thursday:
                ThursdayStart = start;
                ThursdayEnd = end;
                ThursdayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Friday:
                FridayStart = start;
                FridayEnd = end;
                FridayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Saturday:
                SaturdayStart = start;
                SaturdayEnd = end;
                SaturdayBreakMinutes = breakMinutes;
                break;
            case DayOfWeek.Sunday:
                SundayStart = start;
                SundayEnd = end;
                SundayBreakMinutes = breakMinutes;
                break;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Obtém o horário de trabalho de um dia da semana
    /// </summary>
    public (TimeSpan? start, TimeSpan? end, int breakMinutes) GetDaySchedule(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => (MondayStart, MondayEnd, MondayBreakMinutes),
            DayOfWeek.Tuesday => (TuesdayStart, TuesdayEnd, TuesdayBreakMinutes),
            DayOfWeek.Wednesday => (WednesdayStart, WednesdayEnd, WednesdayBreakMinutes),
            DayOfWeek.Thursday => (ThursdayStart, ThursdayEnd, ThursdayBreakMinutes),
            DayOfWeek.Friday => (FridayStart, FridayEnd, FridayBreakMinutes),
            DayOfWeek.Saturday => (SaturdayStart, SaturdayEnd, SaturdayBreakMinutes),
            DayOfWeek.Sunday => (SundayStart, SundayEnd, SundayBreakMinutes),
            _ => (null, null, 0)
        };
    }

    /// <summary>
    /// Verifica se o dia da semana é dia de trabalho
    /// </summary>
    public bool IsWorkDay(DayOfWeek dayOfWeek)
    {
        var (start, end, _) = GetDaySchedule(dayOfWeek);
        return start.HasValue && end.HasValue;
    }

    /// <summary>
    /// Calcula horas esperadas para um dia
    /// </summary>
    public TimeSpan GetExpectedHours(DayOfWeek dayOfWeek)
    {
        var (start, end, breakMinutes) = GetDaySchedule(dayOfWeek);
        
        if (!start.HasValue || !end.HasValue)
            return TimeSpan.Zero;

        var totalMinutes = (end.Value - start.Value).TotalMinutes - breakMinutes;
        return TimeSpan.FromMinutes(Math.Max(0, totalMinutes));
    }

    public void UpdateInfo(string name, string? description, decimal weeklyHours, int toleranceMinutes)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Name = name;

        Description = description;

        if (weeklyHours > 0 && weeklyHours <= 60)
            WeeklyHours = weeklyHours;

        if (toleranceMinutes >= 0 && toleranceMinutes <= 60)
            ToleranceMinutes = toleranceMinutes;

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
}

/// <summary>
/// Tipo de jornada de trabalho
/// </summary>
public enum WorkScheduleType
{
    /// <summary>
    /// Horário fixo (ex: 9h-18h todo dia)
    /// </summary>
    Fixed = 0,
    
    /// <summary>
    /// Horário flexível (banco de horas)
    /// </summary>
    Flexible = 1,
    
    /// <summary>
    /// Turnos alternados (ex: escala 12x36)
    /// </summary>
    Shift = 2
}