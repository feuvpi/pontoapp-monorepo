namespace PontoAPP.Domain.Enums;

/// <summary>
/// Tipo de registro de ponto (entrada/saída)
/// </summary>
public enum RecordType
{
    /// <summary>
    /// Entrada - início do expediente ou retorno de intervalo
    /// </summary>
    ClockIn = 1,
    
    /// <summary>
    /// Saída - fim do expediente ou início de intervalo
    /// </summary>
    ClockOut = 2
}