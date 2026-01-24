namespace PontoAPP.Domain.Models.Reports;

/// <summary>
/// Model para geração do CRP - Comprovante de Registro de Ponto (PDF)
/// </summary>
public class CRPModel
{
    public EmpresaInfo Empresa { get; set; } = new();
    public FuncionarioInfo Funcionario { get; set; } = new();
    public RegistroInfo Registro { get; set; } = new();
    
    public class EmpresaInfo
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public string CNPJ { get; set; } = string.Empty;
    }
    
    public class FuncionarioInfo
    {
        public string Nome { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string PIS { get; set; } = string.Empty;
    }
    
    public class RegistroInfo
    {
        public DateTime DataHora { get; set; }
        public string Tipo { get; set; } = string.Empty; // ENTRADA/SAÍDA
        public long NSR { get; set; }
        public string Hash { get; set; } = string.Empty;
        public string DispositivoIP { get; set; } = string.Empty;
        public string Localizacao { get; set; } = string.Empty;
    }
}