namespace PontoAPP.Domain.Models.Reports;

/// <summary>
/// Model para geração do Espelho de Ponto mensal (PDF)
/// </summary>
public class EspelhoModel
{
    public EmpregadorInfo Empregador { get; set; } = new();
    public FuncionarioInfo Funcionario { get; set; } = new();
    public int Ano { get; set; }
    public int Mes { get; set; }
    public List<DiaRegistro> Registros { get; set; } = new();
    public ResumoMensal Resumo { get; set; } = new();
    
    public class EmpregadorInfo
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
    }
    
    public class FuncionarioInfo
    {
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string PIS { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public DateTime DataAdmissao { get; set; }
    }
    
    public class DiaRegistro
    {
        public DateTime Data { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public List<Marcacao> Marcacoes { get; set; } = new();
        public string TotalHoras { get; set; } = string.Empty;
        public string HorasExtras { get; set; } = string.Empty;
    }
    
    public class Marcacao
    {
        public string Hora { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // ENTRADA/SAÍDA
        public long NSR { get; set; }
        public string Origem { get; set; } = string.Empty; // WEB/MOBILE/MANUAL
    }
    
    public class ResumoMensal
    {
        public int DiasTrabalhadosPrevistos { get; set; }
        public int DiasTrabalhados { get; set; }
        public int Faltas { get; set; }
        public string TotalHorasTrabalhadas { get; set; } = string.Empty;
        public string TotalHorasExtras { get; set; } = string.Empty;
        public string TotalHorasNoturnas { get; set; } = string.Empty;
    }
}