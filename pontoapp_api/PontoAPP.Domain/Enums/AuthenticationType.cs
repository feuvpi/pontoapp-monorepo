namespace PontoAPP.Domain.Enums;

/// <summary>
/// Método de autenticação usado para registrar o ponto
/// </summary>
public enum AuthenticationType
{
    /// <summary>
    /// Biometria do dispositivo móvel (digital, face)
    /// </summary>
    Biometric = 1,
    
    /// <summary>
    /// Senha do usuário
    /// </summary>
    Password = 2,
    
    /// <summary>
    /// PIN numérico (para terminais compartilhados)
    /// </summary>
    Pin = 3
}