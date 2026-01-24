namespace PontoAPP.Application.DTOs.Reports;

public class ACJEFResponse
{
    public EmployerData Empregador { get; set; } = new();
    public string Competencia { get; set; } = string.Empty; // yyyy-MM
    public List<EmployeeData> Empregados { get; set; } = new();
}

public class EmployerData
{
    public string CNPJ { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
}

public class EmployeeData
{
    public string CPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string PIS { get; set; } = string.Empty;
    public List<DailyRecord> Registros { get; set; } = new();
}

public class DailyRecord
{
    public string Data { get; set; } = string.Empty; // yyyy-MM-dd
    public List<TimeStamp> Marcacoes { get; set; } = new();
    public WorkSummary Jornada { get; set; } = new();
}

public class TimeStamp
{
    public string Hora { get; set; } = string.Empty; // HH:mm
    public string Tipo { get; set; } = string.Empty; // entrada/saida
    public string NSR { get; set; } = string.Empty; // Número sequencial
}

public class WorkSummary
{
    public string HorasTrabalhadas { get; set; } = "00:00";
    public string HorasExtras { get; set; } = "00:00";
    public string HorasNoturnas { get; set; } = "00:00";
}