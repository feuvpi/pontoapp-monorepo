namespace PontoAPP.Domain.Enums;

/// <summary>
/// Representa os papéis/permissões dos usuários dentro de um tenant
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Administrador do tenant - acesso total às configurações
    /// </summary>
    Admin = 1,
    
    /// <summary>
    /// Gerente - pode gerenciar usuários e visualizar relatórios
    /// </summary>
    Manager = 2,
    
    /// <summary>
    /// Funcionário comum - apenas registra ponto e vê seus próprios registros
    /// </summary>
    Employee = 3
}