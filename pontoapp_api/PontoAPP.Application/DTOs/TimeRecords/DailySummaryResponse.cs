namespace PontoAPP.Application.DTOs.TimeRecords;

public class DailySummaryResponse
{
    public DateTime Date { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public List<TimeRecordResponse> Records { get; set; } = new();
    public TimeSpan TotalWorkedTime { get; set; }
    public string TotalWorkedFormatted => $"{(int)TotalWorkedTime.TotalHours:D2}:{TotalWorkedTime.Minutes:D2}";
    public int TotalRecords { get; set; }
    public bool IsComplete { get; set; } // true se tem pares de entrada/saída completos
    public string? CurrentStatus { get; set; } // "Trabalhando", "Fora", "Intervalo"
}