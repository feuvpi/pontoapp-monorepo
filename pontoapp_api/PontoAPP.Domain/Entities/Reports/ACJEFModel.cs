// PontoAPP.Domain/Models/Reports/ACJEFModel.cs
namespace PontoAPP.Domain.Models.Reports;

public class ACJEFModel
{
    public EmployerInfo Empregador { get; set; } = new();
    public string Competencia { get; set; } = string.Empty;
    public List<EmployeeInfo> Empregados { get; set; } = new();
}

public class EmployerInfo
{
    public string CNPJ { get; set; } = string.Empty;
    public string RazaoSocial { get; set; } = string.Empty;
}

public class EmployeeInfo
{
    public string CPF { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string PIS { get; set; } = string.Empty;
    public List<DailyRecordInfo> Registros { get; set; } = new();
}

public class DailyRecordInfo
{
    public string Data { get; set; } = string.Empty;
    public List<TimeStampInfo> Marcacoes { get; set; } = new();
    public WorkSummaryInfo Jornada { get; set; } = new();
}

public class TimeStampInfo
{
    public string Hora { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string NSR { get; set; } = string.Empty;
}

public class WorkSummaryInfo
{
    public string HorasTrabalhadas { get; set; } = "00:00";
    public string HorasExtras { get; set; } = "00:00";
    public string HorasNoturnas { get; set; } = "00:00";
}